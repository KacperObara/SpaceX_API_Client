using Cysharp.Threading.Tasks;
using Data;
using Modules.PayloadPopupModule;
using States;
using VContainer;

namespace Modules.LaunchesModule
{
    public class LaunchesState : State
    {
        private readonly ILaunchesPresenter _presenter;
        private readonly PayloadPopupLifetimeScope _popupPrefab;
        
        public LaunchesState(
            StateMachine machine,
            ILaunchesPresenter presenter,
            PayloadPopupLifetimeScope popupPrefab) : base(machine)
        {
            _presenter = presenter;
            _popupPrefab = popupPrefab;
        }

        public override async UniTask Enter()
        {
            _presenter.PopupRequested += OnPopupRequested;
            _presenter.ExitRequested += OnExitRequested;
            await _presenter.Show();
        }
        
        public override async UniTask Exit()
        {
            _presenter.PopupRequested -= OnPopupRequested;
            _presenter.ExitRequested -= OnExitRequested;
            await _presenter.Hide();
        }
        
        private async UniTask OnPopupRequested(LaunchData data)
        {
            await Machine.Push<PayloadPopupState>(Scope, _popupPrefab, builder =>
            {
                builder.RegisterInstance(data);
            });
        }
        
        private async UniTask OnExitRequested()
        {
            await Machine.Pop();
        }
        
        public override void Dispose()
        {
            _presenter.PopupRequested -= OnPopupRequested;
            _presenter.ExitRequested -= OnExitRequested;
            _presenter.Dispose();
            base.Dispose();
        }
    }
}
