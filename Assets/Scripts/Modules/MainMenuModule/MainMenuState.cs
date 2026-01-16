using Cysharp.Threading.Tasks;
using Modules.LaunchesModule;
using Modules.SolarSimulationModule;
using States;
using UnityEngine;

namespace Modules.MainMenuModule
{
    public class MainMenuState : State
    {
        private readonly IMainMenuPresenter _presenter;
        private readonly SolarSimulationLifetimeScope _simulationPrefab;
        private readonly LaunchesLifetimeScope _launchesPrefab;

         public MainMenuState(
             StateMachine machine,
             IMainMenuPresenter presenter,
             SolarSimulationLifetimeScope simulationPrefab,
             LaunchesLifetimeScope launchesPrefab) : base(machine)
         {
             _presenter = presenter;
             _simulationPrefab = simulationPrefab;
             _launchesPrefab = launchesPrefab;
         }
        
         public override async UniTask Enter()
         {
             _presenter.SimulationRequested += EnterSolarSimulation;
             _presenter.LaunchesRequested += EnterLaunches;
             await _presenter.Show();
         }

         public override async UniTask Exit()
         {
             Debug.LogError("MainMenu should never exit during normal play");
             
             _presenter.SimulationRequested -= EnterSolarSimulation;
             _presenter.LaunchesRequested -= EnterLaunches;
            
             await _presenter.Hide();
         }
         
         public override UniTask OnSuspend()
         {
             _presenter.Hide();
             return UniTask.CompletedTask;
         }
         
         public override UniTask OnResume()
         {
             _presenter.Show();
             return UniTask.CompletedTask;
         }
         
         private async UniTask EnterSolarSimulation()
         {
             await Machine.Push<SolarSimulationState>(Scope, _simulationPrefab);
         }

         private async UniTask EnterLaunches()
         {
             await Machine.Push<LaunchesState>(Scope, _launchesPrefab);
         }

         public override void Dispose()
         {
             _presenter.SimulationRequested -= EnterSolarSimulation;
             _presenter.LaunchesRequested -= EnterLaunches;
             _presenter.Dispose();
             base.Dispose();
         }
    }
}