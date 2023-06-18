using System.Collections;
using System.Text.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tezos.StarterScene
{
    public class TestTransferFA12Token : MonoBehaviour
    {
        [Header("References")] 
        [SerializeField] private Button _button;
        [SerializeField] private Button _hyperlinkButton;
        [SerializeField] private TextMeshProUGUI _contractAddressText;
        [SerializeField] private TextMeshProUGUI _resultText;
        [SerializeField] private TMP_InputField _inputFieldAddress;
        [SerializeField] private TMP_InputField _inputFieldAmount;

        private void Start()
        {
            _button.onClick.AddListener(OnTransferFA12TokenButtonClicked);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnTransferFA12TokenButtonClicked);
        }

        private void OnTransferFA12TokenButtonClicked()
        {
            _resultText.text = "Requested.";
            _hyperlinkButton.interactable = false;

            // TODO:
        }
        
        private void OnContractCallInjected(string transaction)
        {
            TezosManager.Instance.MessageReceiver.ContractCallInjected -= OnContractCallInjected;
            var json = JsonSerializer.Deserialize<JsonElement>(transaction);
            var transactionHash = json.GetProperty("transactionHash").GetString();
            IEnumerator routine = TezosManager.Instance.TrackTransaction(transactionHash, result =>
            {
                if (result.success)
                {
                    _resultText.text = result.transactionHash;
                    _hyperlinkButton.interactable = true;
                }
                else
                {
                    _resultText.text = "Failed.";
                }
            });
            TezosManager.Instance.StartCoroutine(routine);
            _resultText.text = "Pending...";
        }
    }
}
