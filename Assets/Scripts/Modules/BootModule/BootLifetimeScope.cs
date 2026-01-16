using Data;
using Modules.LoadingErrorModule;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.BootModule
{
    public class BootLifetimeScope : LifetimeScope
    {
        [SerializeField] private LoadingErrorLifetimeScope _loadingErrorModulePrefab;
    
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<BootState>(Lifetime.Transient);
            builder.Register<LoadingErrorState>(Lifetime.Scoped);
        
            builder.RegisterComponentInHierarchy<BootView>()
                .AsImplementedInterfaces();
        
            builder.RegisterInstance(_loadingErrorModulePrefab);
        
            builder.Register<IOrbitalDataLoader, OrbitalDataLoader>(Lifetime.Scoped);
            builder.Register<ILaunchDataLoader, LaunchDataLoader>(Lifetime.Scoped);
        }
    }
}
