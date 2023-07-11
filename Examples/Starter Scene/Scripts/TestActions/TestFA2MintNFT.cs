using System.Collections;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tezos.StarterScene
{
    //TODO: Uses the Inventory Sample Game contract to showcase functionality. After the FA2 branch is merged into the main branch, use the FA2 wrapper instead.
    public class TestFA2MintNFT : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _contractAddressText;
        [SerializeField] private TextMeshProUGUI _textTxnHash;
        [SerializeField] private Button _hyperlinkButton;
        
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
            _button.interactable = true;
            _textTxnHash.text = "Requested.";
            _hyperlinkButton.interactable = false;
            
            string entrypoint = "mint";
            string input = "{\"prim\": \"Unit\"}";
            JObject obj = new JObject();
            obj["data"] = JObject.Parse(input);
            string newInput = obj.ToString();
            TezosManager.Instance.MessageReceiver.ContractCallInjected += OnContractCallInjected;
            TezosManager.Instance.CallContract(_contractAddressText.text, entrypoint, newInput, 0);
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