using VContainer;
using VContainer.Unity;

namespace Modules.LoadingErrorModule
{
    public class LoadingErrorLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<LoadingErrorState>(Lifetime.Transient);
        
            builder.RegisterComponentInHierarchy<LoadingErrorView>()
                .AsImplementedInterfaces();
        }
    }
}
