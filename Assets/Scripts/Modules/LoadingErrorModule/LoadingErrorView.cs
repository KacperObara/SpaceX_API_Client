using System;
using Cysharp.Threading.Tasks;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.LoadingErrorModule
{
    public interface ILoadingErrorView
    {
        UniTask Show();
        UniTask Hide();
        event Func<UniTask> RetryClicked;
    }

    public class LoadingErrorView : MonoBehaviour, ILoadingErrorView
    {
        public event Func<UniTask> RetryClicked;
        
        [SerializeField] private Button _retryButton;
        [SerializeField] private CanvasGroup _group;

        private void Awake() => _retryButton.onClick.AddListener(() => RetryClicked?.Invoke());

        public async UniTask Show() => await _group.DOFade(1, 0.1f).AsyncWaitForCompletion();
        public async UniTask Hide() => await _group.DOFade(0, 0.1f).AsyncWaitForCompletion();
    }
}