using Cysharp.Threading.Tasks;
using States;

namespace Modules.LoadingErrorModule
{
    public class LoadingErrorState : State
    {
        private readonly ILoadingErrorView _view;
        
        public LoadingErrorState(
            StateMachine machine, 
            ILoadingErrorView view) : base(machine)
        {
            _view = view;
        }
        
        public override async UniTask Enter()
        {
            _view.RetryClicked += OnRetry;
            await _view.Show();
        }

        private async UniTask OnRetry()
        {
            await Machine.Pop();
        }

        public override async UniTask Exit()
        {
            _view.RetryClicked -= OnRetry;
            await _view.Hide();
        }
    }
}