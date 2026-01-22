using Cysharp.Threading.Tasks;
using Data;
using States;

namespace Modules.PayloadPopupModule
{
    public class PayloadPopupState : State
    {
        private readonly LaunchData _data;
        private readonly IPayloadPopupPresenter _presenter;

        public PayloadPopupState(
            IPayloadPopupPresenter presenter,
            LaunchData data)
        {
            _presenter = presenter;
            _data = data;
        }

        public override async UniTask Enter()
        {
            _presenter.ExitRequested += OnExitRequested;
            await _presenter.Show(_data);
        }
    
        public override async UniTask Exit()
        {
            _presenter.ExitRequested -= OnExitRequested;
            await _presenter.Hide();
        }
    
        private async UniTask OnExitRequested()
        {
            RequestPop();
            await UniTask.CompletedTask;
        }
        
        public override void Dispose()
        {
            _presenter.ExitRequested -= OnExitRequested;
            _presenter.Dispose();
            base.Dispose();
        }
    }
}
