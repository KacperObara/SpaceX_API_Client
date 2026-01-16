using Data;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

// Represents a single launch entry in the UI list of launches
namespace UI
{
    public class LaunchEntry : MonoBehaviour
    {
        public RectTransform RectTransform;
        public Button EnterPopupButton;
    
        [SerializeField] private TextMeshProUGUI _missionNameText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
        [SerializeField] private Image _rocketIcon;
        [SerializeField] private Image _missingRocketIcon;
        [SerializeField] private Image _launchTimeIcon;
        [SerializeField] private Sprite _upcomingSprite;
        [SerializeField] private Sprite _pastSprite;

        
        public void Load(LaunchData launchData)
        {
            _missingRocketIcon.gameObject.SetActive(false);
            _rocketIcon.gameObject.SetActive(false);
        
            _missionNameText.text = launchData.Name;
            _descriptionText.text = $"{launchData.PayloadIds.Count} payload(s)\n{launchData.Rocket.Name}\n({launchData.Rocket.Country})";
        
            _launchTimeIcon.sprite = launchData.Upcoming ? _upcomingSprite : _pastSprite;
            
            Sprite rocketSprite = launchData.Rocket.CachedImage;
            if (rocketSprite == null)
            {
                _missingRocketIcon.gameObject.SetActive(true);
            }
            else
            {
                _rocketIcon.sprite = rocketSprite;
                _rocketIcon.gameObject.SetActive(true);
            }
        }
    }
}


