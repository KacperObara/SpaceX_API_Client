using System;
using System.Collections.Generic;
using System.Threading;
using Cysharp.Threading.Tasks;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace States
{
    public class StateMachine : ITickable
    {
        private readonly Stack<State> _stack = new();
        
        // Global quit token Triggers when app quits. Useful to stop any loading still in progress,
        // because UniTask is not tied to gameobjects like coroutines
        private CancellationToken _appToken; 

        // The game entry point
        public async UniTask PushFirst(State state, LifetimeScope scope, CancellationToken appToken)
        {
            _appToken = appToken;
            state.SetScope(scope);
            _stack.Push(state);
            await state.Enter();
        }
        
        // Custom update loop, allows scripts to be tied to states instead of gameobjects. 
        // When we leave states, they automatically stop updating
        public void Tick()
        {
            if (_stack.TryPeek(out State current))
                current.Tick();
        }
        
        // extraRegistrations is useful when we want to pass custom data to the child state. See LaunchesState
        public async UniTask Push<TState>(
            LifetimeScope parentScope, 
            LifetimeScope prefab, 
            Action<IContainerBuilder> extraRegistrations = null) where TState : State
        {
            try 
            {
                if (_stack.TryPeek(out var parent)) 
                    await parent.OnSuspend();
                    
                // We create the source here so we can give it to the Container AND the State
                // Why? When app quits, all states cancel. When this state pops, only this token cancels.
                var stateCts = CancellationTokenSource.CreateLinkedTokenSource(_appToken);

                if (prefab == null) 
                    throw new Exception($"Prefab for {typeof(TState).Name} is null!");
                    
                // Create a child scope from the prefab
                var childScope = parentScope.CreateChildFromPrefab(prefab, builder =>
                {
                    // Register the cancellation token so it can be injected into services
                    builder.RegisterInstance(stateCts.Token); 
                    
                    // This allows us to inject custom data to child states
                    extraRegistrations?.Invoke(builder); 
                });
                    
                if (childScope == null || childScope.Container == null)
                    throw new Exception("VContainer failed to create child scope or container.");

                // Ask VContainer to create the state with all dependencies injected
                var newState = childScope.Container.Resolve<TState>();
                    
                // Give the state references to its scope and token source
                newState.SetScope(childScope);
                // We pass the source we just created so the State can cancel it on Dispose
                newState.AssignTokenSource(stateCts); 
            
                _stack.Push(newState);
                await newState.Enter();
            }
            catch (Exception e) 
            { 
                Debug.LogError($"[StateMachine] Push<{typeof(TState).Name}> Failed: {e}");
                throw;
            }
        }
        
        public async UniTask Pop()
        {
            try
            {
                if (_stack.Count <= 1) 
                    return;

                var poppedState = _stack.Pop();
                await poppedState.Exit();
                poppedState.Dispose(); // This triggers the token cancellation

                if (_stack.TryPeek(out var parent))
                    await parent.OnResume();
            }
            catch (Exception e)
            {
                Debug.LogError($"[StateMachine] Pop Failed: {e}");
                throw;
            }
        }
    }
    
    public abstract class State : IDisposable
    {
        protected readonly StateMachine Machine;
        protected LifetimeScope Scope;
        
        private CancellationTokenSource _cts;
        private bool _disposed;
        
        public State(StateMachine machine) => Machine = machine;

        public void SetScope(LifetimeScope scope) => Scope = scope;
        
        public void AssignTokenSource(CancellationTokenSource cts) => _cts = cts;
        
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