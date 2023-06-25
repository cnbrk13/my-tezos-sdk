using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tezos.StarterScene
{
    public class TestGetAccountAddressTezosBalance : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _resultText;
        [SerializeField] private TMP_InputField _inputField;

        private void Start()
        {
            _button.onClick.AddListener(OnButtonClicked);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            _button.interactable = false;
            _resultText.text = "Pending...";

            string address = _inputField.text;
            var routine = TezosManager.Instance.GetTezosBalance(balance =>
            {
                double doubleBalance = balance / 1e6; // 6 decimals
                _resultText.text = doubleBalance.ToString();
                _button.interactable = true;
            }, address);
            CoroutineRunner.Instance.StartWrappedCoroutine(routine);
        }
    }
}
