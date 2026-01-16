using System;
using System.Collections.Generic;
using Cysharp.Threading.Tasks;
using Data;
using DG.Tweening;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.UI;

namespace Modules.PayloadPopupModule
{
    public interface IPayloadPopupView
    {
        UniTask Show();
        UniTask Hide();
        event Action ReturnClicked;
        void Load(string missionName, List<PayloadData> payloads);
    }

    public class PayloadPopupView : MonoBehaviour, IPayloadPopupView
    {
        public event Action ReturnClicked;
    
        [SerializeField] private Button _returnButton;
        [SerializeField] private RectTransform _content;
        [SerializeField] private ScrollRect _scrollRect;
    
        [SerializeField] private PayloadEntry _payloadEntryPrefab;
        [SerializeField] private TextMeshProUGUI _missionNameText;
        [SerializeField] private GameObject _noPayloadsText;
        [SerializeField] private Transform _popupParent;

        private readonly List<PayloadEntry> _payloadEntries = new List<PayloadEntry>();
        
        private void Awake()
        {
            _returnButton.onClick.AddListener(() => ReturnClicked?.Invoke());
        }
        
        public UniTask Show()
        {
            return UniTask.CompletedTask;
        }

        public UniTask Hide()
        {
            return UniTask.CompletedTask;
        }
    
        public void Load(string missionName, List<PayloadData> payloads)
        {
            _missionNameText.text = missionName;
            _noPayloadsText.SetActive(false);
        
            _popupParent.localScale = Vector3.zero;
            _popupParent.DOScale(1, 0.3f).SetEase(Ease.OutBack);
        
            LoadPayloadEntries(payloads);
        
            if (payloads.Count == 0)
            {
                _noPayloadsText.SetActive(true);
            }

            ResetScrollPosition();
        }
    
        private void LoadPayloadEntries(List<PayloadData> payloads)
        {
            ClearEntries();

            for (int i = 0; i < payloads.Count; i++)
            {
                PayloadEntry entry = Instantiate(_payloadEntryPrefab, _content.transform); 
                _payloadEntries.Add(entry);
                entry.Load(payloads[i]);
            }
        
            _content.sizeDelta = new Vector2(_content.sizeDelta.x, _payloadEntries.Count * 305f);
        }
    
        public void ResetScrollPosition()
        {
            _scrollRect.verticalNormalizedPosition = 1f;
        }

        private void ClearEntries()
        {
            for (int i = _payloadEntries.Count - 1; i >= 0; i--)
            {
                Destroy(_payloadEntries[i].gameObject);
            }
            _payloadEntries.Clear();
        }
    }
}