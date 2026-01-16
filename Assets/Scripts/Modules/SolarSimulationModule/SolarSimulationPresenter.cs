using System;
using Cysharp.Threading.Tasks;

namespace Modules.SolarSimulationModule
{
    public interface ISolarSimulationPresenter
    {
        UniTask Hide();
        
        event Func<UniTask> ExitRequested;
        public void Dispose();
    }
    
    public class SolarSimulationPresenter : ISolarSimulationPresenter
    {
        public event Func<UniTask> ExitRequested;
        
        private readonly ISolarSimulationView _view;

        public SolarSimulationPresenter(ISolarSimulationView view)
        {
            _view = view;
            _view.ReturnClicked += OnReturnClicked;
        }

        private void OnReturnClicked() => ExitRequested?.Invoke();
        
        public async UniTask Hide() => await _view.Hide();
    
        public void Dispose()
        {
            _view.ReturnClicked -= OnReturnClicked;
        }
    }
}
