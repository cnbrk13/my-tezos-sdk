using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tezos.StarterScene
{
    public class TestGetFA12TotalSupply : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _contractAddressText;
        [SerializeField] private TextMeshProUGUI _resultText;

        private void Start()
        {
            _button.onClick.AddListener(OnGetFA12TotalSupplyButtonClicked);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnGetFA12TotalSupplyButtonClicked);
        }

        private void OnGetFA12TotalSupplyButtonClicked()
        {
            _resultText.text = "Pending...";
            
            // TODO:
        }
    }
}
