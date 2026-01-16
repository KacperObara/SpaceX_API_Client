using Data;
using Modules.BootModule;
using Modules.MainMenuModule;
using States;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.RootModule
{
    public class RootLifetimeScope : LifetimeScope
    {
        [SerializeField] private BootLifetimeScope _bootModulePrefab;
        [SerializeField] private MainMenuLifetimeScope _mainMenuModulePrefab;
        [SerializeField] private Camera _camera;

        protected override void Configure(IContainerBuilder builder)
        {
            builder.RegisterEntryPoint<StateMachine>(Lifetime.Singleton).AsSelf();
            builder.RegisterEntryPoint<GameBootstrapper>();
            builder.Register<RootState>(Lifetime.Singleton);
        
            builder.RegisterInstance(_bootModulePrefab);
            builder.RegisterInstance(_mainMenuModulePrefab);
            builder.RegisterInstance(_camera);
        
            // Persistent game data
            builder.Register<IOrbitalDataService, OrbitalDataService>(Lifetime.Singleton);
            builder.Register<ILaunchDataService, LaunchDataService>(Lifetime.Singleton);
        }
    }
}


