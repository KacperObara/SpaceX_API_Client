using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.LaunchesModule
{
    public interface ILaunchesView
    {
        UniTask Show();
        UniTask Hide();
        event Action ReturnClicked;
        void InitializeList(int totalCount);
        void Bind(ILaunchesPresenter presenter);
    }

    public class LaunchesView : MonoBehaviour, ILaunchesView
    {
        public event Action ReturnClicked;

        [SerializeField] private VerticalLayoutGroup _verticalLayoutGroup;
        [SerializeField] private RectTransform _content;
        [SerializeField] private LaunchEntry _itemPrefab;
        [SerializeField] private ScrollRect _scrollRect;
        [SerializeField] private Button _returnButton;

        private readonly List<LaunchEntry> _entries = new();
        private float _itemHeight;
        private int _visibleCount;
        private int _totalCount; // How many entries are in the whole list (launches)
        private int _currentStartIndex;
        private bool _initialized;
        private ILaunchesPresenter _presenter;

        public void Bind(ILaunchesPresenter presenter)
        {
            _presenter = presenter;
        }

        private void Awake()
        {
            _returnButton.onClick.AddListener((() => ReturnClicked?.Invoke()));
        }

        public UniTask Show()
        {
            return UniTask.CompletedTask;
        }

        public UniTask Hide()
        {
            return UniTask.CompletedTask;
        }

        public void InitializeList(int totalCount)
        {
            if (_initialized)
                return;

            _totalCount = totalCount;

            // Calculate how many items can fit in the viewport
            // and instantiate only that many plus a small buffer
            _itemHeight = _itemPrefab.RectTransform.rect.height + _verticalLayoutGroup.spacing;
            _visibleCount = Mathf.CeilToInt(_scrollRect.viewport.rect.height / _itemHeight) + 3;

            for (int i = 0; i < _visibleCount; i++)
            {
                var entry = Instantiate(_itemPrefab, _content);
                _entries.Add(entry);
            }

            // Set content height based on total items
            _content.sizeDelta = new Vector2(_content.sizeDelta.x, _totalCount * _itemHeight);
            ResetScrollPosition();

            _initialized = true;
            UpdateVisibleEntries();
        }

        private void Update()
        {
            if (!_initialized)
                return;

            int newIndex = Mathf.FloorToInt(_scrollRect.content.anchoredPosition.y / _itemHeight);
            if (newIndex != _currentStartIndex)
            {
                int maxStart = Mathf.Max(0, _totalCount - _visibleCount);
                _currentStartIndex = Mathf.Clamp(newIndex, 0, maxStart);

                UpdateVisibleEntries();
            }
        }

        
        // Dynamically load entries based on scroll position.
        // Thanks to that, we can have infinite number of entries with just ~5-10 UI Gameobjects.
        private void UpdateVisibleEntries()
        {
            for (int i = 0; i < _entries.Count; i++)
            {
                int dataIndex = _currentStartIndex + i;

                if (dataIndex >= 0 && dataIndex < _totalCount)
                {
                    _entries[i].gameObject.SetActive(true);
                    _entries[i].RectTransform.anchoredPosition =
                        new Vector2(_entries[i].RectTransform.anchoredPosition.x, -dataIndex * _itemHeight);

                    var data = _presenter.GetData(dataIndex);
                    _entries[i].Load(data);

                    // Remove previous listeners and forward clicks to presenter
                    _entries[i].EnterPopupButton.onClick.RemoveAllListeners();
                    _entries[i].EnterPopupButton.onClick.AddListener(() =>
                    {
                        var d = _presenter.GetData(dataIndex);
                        _presenter.OnEntryClicked(d);
                    });
                }
                else
                {
                    _entries[i].gameObject.SetActive(false);
                }
            }
        }

        public void ResetScrollPosition()
        {
            _scrollRect.verticalNormalizedPosition = 1f;
            _currentStartIndex = 0;
        }
    }
}