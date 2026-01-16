using System;
using Cysharp.Threading.Tasks;

namespace Modules.MainMenuModule
{
    public interface IMainMenuPresenter : IDisposable
    {
        UniTask Show();
        UniTask Hide();
        
        public event Func<UniTask> SimulationRequested;
        public event Func<UniTask> LaunchesRequested;
    }
    
    public class MainMenuPresenter : IMainMenuPresenter
    {
        public event Func<UniTask> SimulationRequested;
        public event Func<UniTask> LaunchesRequested;
        
        private readonly IMainMenuView _view;
        
        public MainMenuPresenter(IMainMenuView view)
        {
            _view = view;
            
            _view.SimulationClicked += OnSimulationClicked;
            _view.LaunchesClicked += OnLaunchesClicked;
        }
        
        public async UniTask Show() => await _view.Show();
        public async UniTask Hide() => await _view.Hide();

        private void OnSimulationClicked() => SimulationRequested?.Invoke();
        private void OnLaunchesClicked() => LaunchesRequested?.Invoke();

        public void Dispose()
        {
            _view.SimulationClicked -= OnSimulationClicked;
            _view.LaunchesClicked -= OnLaunchesClicked;
        }
    }
}