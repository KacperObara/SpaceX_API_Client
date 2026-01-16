using System.Threading;
using Modules.RootModule;
using UnityEngine;
using VContainer.Unity;

namespace States
{
    public class GameBootstrapper : IAsyncStartable
    {
        private readonly StateMachine _stateMachine;
        private readonly RootLifetimeScope _rootScope;
        private readonly RootState _rootState;
        
        public GameBootstrapper(
            StateMachine stateMachine, 
            RootLifetimeScope rootScope,
            RootState rootState)
        {
            _stateMachine = stateMachine;
            _rootScope = rootScope;
            _rootState = rootState;
        }

        public async Awaitable StartAsync(CancellationToken cancellation)
        {
            await _stateMachine.PushFirst(_rootState, _rootScope, cancellation);
        }
    }
}
