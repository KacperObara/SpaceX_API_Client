using VContainer;
using VContainer.Unity;

namespace Modules.PayloadPopupModule
{
    public class PayloadPopupLifetimeScope : LifetimeScope
    {
        protected override void Configure(IContainerBuilder builder)
        {
            builder.Register<PayloadPopupState>(Lifetime.Transient);
            builder.Register<IPayloadPopupPresenter, PayloadPopupPresenter>(Lifetime.Scoped);
        
            builder.RegisterComponentInHierarchy<PayloadPopupView>()
                .AsImplementedInterfaces();
        }
    }
}
