using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tezos.StarterScene
{
    public class TestGetAccountAddressFA12Balance : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _contractAddressText;
        [SerializeField] private TextMeshProUGUI _resultText;
        [SerializeField] private TMP_InputField _inputField;

        private void Start()
        {
            _button.onClick.AddListener(OnGetAccountAddressFA12BalanceButtonClicked);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnGetAccountAddressFA12BalanceButtonClicked);
        }

        private void OnGetAccountAddressFA12BalanceButtonClicked()
        {
            _button.interactable = false;
            _resultText.text = "Pending...";
            
            // TODO:
        }
    }
}
