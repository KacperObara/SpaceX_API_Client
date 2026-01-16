using Modules.LaunchesModule;
using Modules.SolarSimulationModule;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.MainMenuModule
{
    public class MainMenuLifetimeScope : LifetimeScope
    {
        [SerializeField] private SolarSimulationLifetimeScope _simulationPrefab;
        [SerializeField] private LaunchesLifetimeScope _launchesPrefab;
    
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<MainMenuState>(Lifetime.Transient);
            builder.Register<IMainMenuPresenter, MainMenuPresenter>(Lifetime.Scoped);
        
            builder.RegisterComponentInHierarchy<MainMenuView>()
                .AsImplementedInterfaces();
        
            builder.RegisterInstance(_simulationPrefab);
            builder.RegisterInstance(_launchesPrefab);
        }
    }
}
