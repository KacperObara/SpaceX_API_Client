using Data;
using TMPro;
using UnityEngine;

// Represents a single ship entry in the UI list when clicking on a launch
namespace UI
{
    public class PayloadEntry : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI _payloadNameText;
        [SerializeField] private TextMeshProUGUI _descriptionText;
    
        private PayloadData _data;
    
        public void Load(PayloadData payloadData)
        {
            _data = payloadData;
            
            string mass = string.IsNullOrWhiteSpace(payloadData.Mass) 
                ? "Unknown Weight" 
                : $"{payloadData.Mass} kg";

            _payloadNameText.text = _data.Name;
            _descriptionText.text =  $"{payloadData.Type}" +
                                   $"\n{mass}" +
                                   $"\n{string.Join(", ", payloadData.Customers)}";
        }
    }
}


