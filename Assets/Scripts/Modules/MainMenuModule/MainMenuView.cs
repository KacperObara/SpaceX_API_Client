using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.MainMenuModule
{
    public interface IMainMenuView
    {
        UniTask Show();
        UniTask Hide();
        event Action SimulationClicked;
        event Action LaunchesClicked;
    }

    public class MainMenuView : MonoBehaviour, IMainMenuView
    {
        public event Action SimulationClicked;
        public event Action LaunchesClicked;
        
        [SerializeField] private Button _simulationButton;
        [SerializeField] private Button _launchesButton;
        [SerializeField] private CanvasGroup _canvasGroup;

        private void Awake()
        {
            _simulationButton.onClick.AddListener(() => SimulationClicked?.Invoke());
            _launchesButton.onClick.AddListener(() => LaunchesClicked?.Invoke());
        }
        
        public UniTask Show()
        {
            _canvasGroup.blocksRaycasts = true;
            _canvasGroup.interactable = true;
            _canvasGroup.alpha = 1f;
            return UniTask.CompletedTask;
        }

        public UniTask Hide()
        {
            _canvasGroup.blocksRaycasts = false;
            _canvasGroup.alpha = 0f;
            return UniTask.CompletedTask;
        }
    }
}