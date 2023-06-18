using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tezos.StarterScene
{
    public class TestGetYourTezosBalance : MonoBehaviour
    {
        [Header("References")] [SerializeField]
        private Button _button;

        [SerializeField] private TextMeshProUGUI _resultText;

        private void Start()
        {
            _button.onClick.AddListener(OnGetYourTezosBalanceButtonClicked);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnGetYourTezosBalanceButtonClicked);
        }

        private void OnGetYourTezosBalanceButtonClicked()
        {
            _resultText.text = "Pending...";

            string address = TezosManager.Instance.GetActiveAddress();
            var routine = TezosManager.Instance.GetTezosBalance(balance =>
            {
                // 6 decimals
                var doubleBalance = balance / 1e6;
                _resultText.text = doubleBalance.ToString();
            }, address);
            CoroutineRunner.Instance.StartWrappedCoroutine(routine);
        }
    }
}
