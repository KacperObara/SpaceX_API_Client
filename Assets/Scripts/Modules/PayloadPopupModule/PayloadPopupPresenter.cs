using System;
using Cysharp.Threading.Tasks;
using Data;

namespace Modules.PayloadPopupModule
{
    public interface IPayloadPopupPresenter
    {
        UniTask Show(LaunchData data);
        UniTask Hide();
        event Func<UniTask> ExitRequested;
        public void Dispose();
    }

    public class PayloadPopupPresenter : IPayloadPopupPresenter
    {
        public event Func<UniTask> ExitRequested;
    
        private readonly IPayloadPopupView _view;
    
        public PayloadPopupPresenter(IPayloadPopupView view)
        {
            _view = view;
            _view.ReturnClicked += OnReturnClicked;
        }
    
        public async UniTask Show(LaunchData data)
        {
            _view.Load(data.Name, data.Payloads);
            await _view.Show();
        }
    
        private void OnReturnClicked() => ExitRequested?.Invoke();
    
        public async UniTask Hide() => await _view.Hide();
    
        public void Dispose()
        {
            _view.ReturnClicked -= OnReturnClicked;
        }
    }
}