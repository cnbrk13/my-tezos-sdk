using System.Collections;
using System.Text.Json;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tezos.StarterScene
{
    public class TestMintNFT : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _contractAddressText;
        [SerializeField] private TextMeshProUGUI _textTxnHash;
        [SerializeField] private Button _hyperlinkButton;
        
        private void Start()
        {
            _button.onClick.AddListener(OnMintNFTButtonClicked);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnMintNFTButtonClicked);
        }
        
        private void OnMintNFTButtonClicked()
        {
            _button.interactable = true;
            _textTxnHash.text = "Requested.";
            _hyperlinkButton.interactable = false;
            
            string entrypoint = "mint";
            string input = "{\"prim\": \"Unit\"}";

            TezosManager.Instance.MessageReceiver.ContractCallInjected += OnContractCallInjected;
            TezosManager.Instance.CallContract(_contractAddressText.text, entrypoint, input, 0);
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
                    _textTxnHash.text = result.transactionHash;
                    _hyperlinkButton.interactable = true;
                }
                else
                {
                    _textTxnHash.text = "Failed.";
                }
            });
            TezosManager.Instance.StartCoroutine(routine);
            _textTxnHash.text = "Pending...";
        }
    }
}