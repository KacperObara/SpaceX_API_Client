using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace States
{
    // A simple data container to tell the Machine what to do next
    public enum TransitionType { Push, Pop }
    
    public struct StateRequest
    {
        public TransitionType Type;
        public Type StateType;
        public LifetimeScope Prefab;
        
        // extraRegistrations is useful when we want to pass custom data to the child state. See LaunchesState
        public Action<IContainerBuilder> ExtraRegistrations;
    }
    
    public class StateMachine : ITickable, IDisposable
    {
        private readonly Stack<State> _stack = new();
        
        // Global quit token Triggers when app quits. Useful to stop any loading still in progress,
        // because UniTask is not tied to gameobjects like coroutines
        private CancellationToken _appToken; 
        
        public event Action<State> OnStateEntered;
        public event Action<State> OnStateExited;

        // The game entry point
        public async UniTask PushFirst(State state, LifetimeScope scope, CancellationToken appToken)
        {
            _appToken = appToken;
            SetupState(state, scope);
            _stack.Push(state);
            
            OnStateEntered?.Invoke(state);
            await state.Enter();
        }
        
        // Custom update loop, allows scripts to be tied to states instead of gameobjects. 
        // When we leave states, they automatically stop updating
        public void Tick()
        {
            if (_stack.TryPeek(out State current))
                current.Tick();
        }
        
        // Give the state references to its scope and token source
        private void SetupState(State state, LifetimeScope scope)
        {
            state.SetScope(scope);
            state.OnTransitionRequested += HandleTransitionRequest;
        }

        // This triggers the token cancellation
        private void CleanupState(State state)
        {
            state.OnTransitionRequested -= HandleTransitionRequest;
            OnStateExited?.Invoke(state);
            state.Dispose();
        }
        
        private void HandleTransitionRequest(StateRequest request)
        {
            switch (request.Type)
            {
                case TransitionType.Push:
                    Push(request).Forget();
                    break;
                case TransitionType.Pop:
                    Pop().Forget();
                    break;
            }
        }
        
        private async UniTaskVoid Push(StateRequest request)
        {
            try
            {
                if (_stack.TryPeek(out State parent))
                    await parent.OnSuspend();

                // We create the source here so we can give it to the Container AND the State
                // Why? When app quits, all states cancel. When this state pops, only this token cancels.
                CancellationTokenSource stateCts = CancellationTokenSource.CreateLinkedTokenSource(_appToken);
                
                // Create a child scope from the prefab
                var childScope = parent.Scope.CreateChildFromPrefab(request.Prefab, builder =>
                {
                    // Register the cancellation token so it can be injected into services
                    builder.RegisterInstance(stateCts.Token);
                    // This allows us to inject custom data to child states
                    request.ExtraRegistrations?.Invoke(builder);
                });
                
                // Ask VContainer to create the state with all dependencies injected
                var newState = (State)childScope.Container.Resolve(request.StateType);
                
                SetupState(newState, childScope);
                
                // We pass the source we just created so the State can cancel it on Dispose
                newState.AssignTokenSource(stateCts);

                _stack.Push(newState);
                OnStateEntered?.Invoke(newState);
                await newState.Enter();
            }
            catch (Exception e)
            {
                Debug.LogError($"[StateMachine] Push Failed: {e}");
            }
        }
        
        public async UniTask Pop()
        {
            try
            {
                if (_stack.Count <= 1) 
                    return;

                State poppedState = _stack.Pop();
                await poppedState.Exit();
                CleanupState(poppedState);

                if (_stack.TryPeek(out var parent))
                    await parent.OnResume();
            }
            catch (Exception e)
            {
                Debug.LogError($"[StateMachine] Pop Failed: {e}");
                throw;
            }
        }
        
        public void Dispose()
        {
            while (_stack.Count > 0)
            {
                CleanupState(_stack.Pop());
            }
        }
    }
    
    public abstract class State : IDisposable
    {
        public event Action<StateRequest> OnTransitionRequested;
        
        public LifetimeScope Scope { get; private set; }
        private CancellationTokenSource _cts;
        private bool _disposed;

        public void SetScope(LifetimeScope scope) => Scope = scope;
        public void AssignTokenSource(CancellationTokenSource cts) => _cts = cts;
        
        protected void RequestPush<T>(LifetimeScope prefab, Action<IContainerBuilder> extra = null) where T : State
        {
            OnTransitionRequested?.Invoke(new StateRequest 
            { 
                Type = TransitionType.Push, 
                StateType = typeof(T), 
                Prefab = prefab, 
                ExtraRegistrations = extra 
            });
        }
        
        protected void RequestPop() 
        {
            OnTransitionRequested?.Invoke(new StateRequest { Type = TransitionType.Pop });
        }
        
        public virtual UniTask Enter() => UniTask.CompletedTask;
        // Called before child is pushed
        public virtual UniTask OnSuspend() => UniTask.CompletedTask;
        // Called after a child is popped
        public virtual UniTask OnResume() => UniTask.CompletedTask;
        public virtual UniTask Exit() => UniTask.CompletedTask;
        public virtual void Tick() { }
        
        public virtual void Dispose()
        {
            if (_disposed) return;
            _disposed = true;
            
            _cts?.Cancel();
            _cts?.Dispose();
            _cts = null;
            
            // VContainer will try to call Dispose() on this state again,
            // but the _disposed flag will stop the recursion.
            var scopeToDispose = Scope;
            Scope = null;
            scopeToDispose?.Dispose();
        }
        
    }
}