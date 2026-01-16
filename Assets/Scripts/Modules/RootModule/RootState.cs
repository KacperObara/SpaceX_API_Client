using Cysharp.Threading.Tasks;
using Modules.BootModule;
using Modules.MainMenuModule;
using States;

namespace Modules.RootModule
{
    public class RootState : State
    {
        private readonly BootLifetimeScope _bootPrefab;
        private readonly MainMenuLifetimeScope _menuPrefab;

        public RootState(
            StateMachine machine,
            BootLifetimeScope bootModulePrefab,
            MainMenuLifetimeScope menuModulePrefab)
            : base(machine)
        {
            _bootPrefab = bootModulePrefab;
            _menuPrefab = menuModulePrefab;
        }
        
        public override async UniTask Enter()
        {
            await Machine.Push<BootState>(Scope, _bootPrefab);
        }

        public override async UniTask OnResume()
        {
            await Machine.Push<MainMenuState>(Scope, _menuPrefab);
        }
    }
}