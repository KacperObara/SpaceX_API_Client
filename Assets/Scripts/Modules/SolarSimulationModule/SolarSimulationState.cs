using Cysharp.Threading.Tasks;
using States;
using UnityEngine;

namespace Modules.SolarSimulationModule
{
    public class SolarSimulationState : State
    {
        private readonly ISolarSimulationPresenter _presenter;
        private readonly SolarSimulator _simulator;
        private readonly CameraControls _cameraControls;
        
        public SolarSimulationState( 
            ISolarSimulationPresenter presenter,
            SolarSimulator simulator,        
            CameraControls cameraControls) 
        {
            _presenter = presenter;
            _simulator = simulator;
            _cameraControls = cameraControls;
        }
        
        public override UniTask Enter()
        {
            _presenter.ExitRequested += OnExitRequested;
            return UniTask.CompletedTask;
        }
        
        public override void Tick()
        {
            _simulator.TickSimulation(Time.deltaTime);
            _cameraControls.TickCamera(Time.deltaTime);
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
