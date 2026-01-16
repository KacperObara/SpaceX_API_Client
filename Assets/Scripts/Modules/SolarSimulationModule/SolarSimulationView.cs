using System;
using Cysharp.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.SolarSimulationModule
{
    public interface ISolarSimulationView
    {
        UniTask Hide();
        event Action ReturnClicked;
    }

    public class SolarSimulationView : MonoBehaviour, ISolarSimulationView
    {
        public event Action ReturnClicked;
        
        [SerializeField] private Button _returnButton;
        [SerializeField] private TimeScaler _timeScaler;

        private void Awake()
        {
            _returnButton.onClick.AddListener(OnReturnButtonClicked);
        }

        public UniTask Hide()
        {
            _timeScaler.ResetTimeScale();
            return UniTask.CompletedTask;
        }

        private void OnReturnButtonClicked() => ReturnClicked?.Invoke();
    }
}