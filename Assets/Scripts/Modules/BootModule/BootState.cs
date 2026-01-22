using System;
using Cysharp.Threading.Tasks;
using Data;
using Modules.LoadingErrorModule;
using States;

// When game launches, Check cache. If not cached, automatically load CSV file and Needed APIs. Also download ships but only one's needed. Then Cache
namespace Modules.BootModule
{
    public class BootState : State
    {
        private readonly IOrbitalDataLoader _orbital;
        private readonly ILaunchDataLoader _launch;
        private readonly IBootView _view;

        private readonly LoadingErrorLifetimeScope _loadingErrorPrefab;
        
        public BootState(
            LoadingErrorLifetimeScope loadingErrorPrefab,
            IBootView view,
            IOrbitalDataLoader orbital,
            ILaunchDataLoader launch)
        {
            _loadingErrorPrefab = loadingErrorPrefab;
            _view = view;
            _orbital = orbital;
            _launch = launch;
        }
        
        public override async UniTask Enter()
        {
            await Load();
        }

        public override async UniTask OnResume()
        {
            await UniTask.Delay(TimeSpan.FromSeconds(1));
            await Load();
        }

        private async UniTask Load()
        {
            try
            {
                _view.SetProgress(0);
                await _orbital.Load();
                _view.SetProgress(20);
                await _launch.Load(p => _view.SetProgress(p));
                RequestPop(); 
            }
            catch (Exception)
            {
                RequestPush<LoadingErrorState>(_loadingErrorPrefab);
            }
        }
    }
}
