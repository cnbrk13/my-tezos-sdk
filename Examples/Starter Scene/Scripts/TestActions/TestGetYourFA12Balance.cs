using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tezos.StarterScene
{
    public class TestGetYourFA12Balance : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _contractAddressText;
        [SerializeField] private TextMeshProUGUI _resultText;

        private void Start()
        {
            _button.onClick.AddListener(OnGetYourFA12BalanceButtonClicked);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnGetYourFA12BalanceButtonClicked);
        }

        private void OnGetYourFA12BalanceButtonClicked()
        {
            _resultText.text = "Pending...";

            // TODO:
        }
    }
}
