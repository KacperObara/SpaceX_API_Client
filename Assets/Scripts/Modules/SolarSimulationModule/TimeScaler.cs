using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.SolarSimulationModule
{
    public class TimeScaler : MonoBehaviour
    {
        [SerializeField] private Slider _slider;
        [SerializeField] private TextMeshProUGUI _timeScaleText;
    
        // Make sure to set timescale back to normal when simulation is unloaded
        public void ResetTimeScale()
        {
            Time.timeScale = 1f;
            _slider.value = 1f;
            OnSliderValueChanged();
        }

        public void OnSliderValueChanged()
        {
            Time.timeScale = _slider.value;
            _timeScaleText.text = $"Time Scale: {Time.timeScale:F0}";
        }
    }
}
