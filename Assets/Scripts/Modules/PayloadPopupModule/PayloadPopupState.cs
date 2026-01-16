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
            StateMachine machine, 
            IPayloadPopupPresenter presenter,
            LaunchData data)
            : base(machine)
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
            await Machine.Pop();
        }
        
        public override void Dispose()
        {
            _presenter.ExitRequested -= OnExitRequested;
            _presenter.Dispose();
            base.Dispose();
        }
    }
}
