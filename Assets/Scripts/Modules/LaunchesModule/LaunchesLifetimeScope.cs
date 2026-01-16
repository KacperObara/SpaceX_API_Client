using Modules.PayloadPopupModule;
using UnityEngine;
using VContainer;
using VContainer.Unity;

namespace Modules.LaunchesModule
{
    public class LaunchesLifetimeScope : LifetimeScope
    {
        [SerializeField] private PayloadPopupLifetimeScope _popupPrefab;
    
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<LaunchesState>(Lifetime.Transient);
            builder.Register<ILaunchesPresenter, LaunchesPresenter>(Lifetime.Scoped);
        
            builder.RegisterComponentInHierarchy<LaunchesView>()
                .AsImplementedInterfaces();
        
            builder.RegisterInstance(_popupPrefab);
        }
    }
}
