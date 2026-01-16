using VContainer;
using VContainer.Unity;

namespace Modules.SolarSimulationModule
{
    public class SolarSimulationLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<SolarSimulationState>(Lifetime.Transient);
            builder.Register<ISolarSimulationPresenter, SolarSimulationPresenter>(Lifetime.Scoped);
        
            builder.RegisterComponentInHierarchy<SolarSimulationView>()
                .AsImplementedInterfaces();
        
            // Components
            builder.RegisterComponentInHierarchy<SolarSimulator>();
            builder.RegisterComponentInHierarchy<CameraControls>();
        }
    }
}
