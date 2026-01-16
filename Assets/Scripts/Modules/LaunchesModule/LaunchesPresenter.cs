using System;
using Cysharp.Threading.Tasks;
using Data;

namespace Modules.LaunchesModule
{
    public interface ILaunchesPresenter : IDisposable
    {
        UniTask Show();
        UniTask Hide();
        
        LaunchData GetData(int dataIndex);
        
        // Looks complicated, but it's just Action<LaunchData>. Func<UniTask> replaces Action when we require awaitable code.
        event Func<LaunchData, UniTask> PopupRequested; 
        event Func<UniTask> ExitRequested;
        
        void OnEntryClicked(LaunchData data);
    }
    
    public class LaunchesPresenter : ILaunchesPresenter
    {
        public event Func<LaunchData, UniTask> PopupRequested;
        public event Func<UniTask> ExitRequested;
        
        private readonly ILaunchesView _view;
        private readonly ILaunchDataService _data;
        
        public LaunchesPresenter(ILaunchesView view, ILaunchDataService data)
        {
            _view = view;
            _data = data;
            _view.ReturnClicked += OnReturnClicked;
        }
    
        public async UniTask Show()
        {
            _view.Bind(this);
            _view.InitializeList(_data.LaunchData.Count);
            await _view.Show();
        }
        
        public LaunchData GetData(int index) => _data.LaunchData[index];

        public void OnEntryClicked(LaunchData data) => PopupRequested?.Invoke(data);

        private void OnReturnClicked() => ExitRequested?.Invoke();

        public async UniTask Hide() => await _view.Hide();

        public void Dispose()
        {
            _view.ReturnClicked -= OnReturnClicked;
        }
    }
}
