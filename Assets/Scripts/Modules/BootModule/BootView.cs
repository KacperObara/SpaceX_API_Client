using DG.Tweening;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.BootModule
{
    public interface IBootView
    {
        void SetProgress(int value);
    }

    public class BootView : MonoBehaviour, IBootView
    {
        [SerializeField] private Slider _progressSlider;
        [SerializeField] private TMP_Text _progressText;
        [SerializeField] private GameObject _loadingIcon;

        public void SetProgress(int value)
        {
            _progressSlider.DOKill(false);
            _progressSlider.DOValue(value, 0.2f);
            _progressText.text = $"{value}%";
        }
    
        private void Update()
        {
            _loadingIcon.transform.Rotate(0, 0, -200 * Time.deltaTime);
        }

        private void OnDestroy()
        {
            _progressSlider.DOKill(false);
        }
    }
}