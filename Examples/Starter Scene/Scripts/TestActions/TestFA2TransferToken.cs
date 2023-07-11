using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using Netezos.Encoding;
using Netezos.Encoding.Serialization;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using JsonSerializer = System.Text.Json.JsonSerializer;

namespace Tezos.StarterScene
{
    //TODO: Uses the Inventory Sample Game contract to showcase functionality. After the FA2 branch is merged into the main branch, use the FA2 wrapper instead.
    public class TestFA2TransferToken : MonoBehaviour
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
            _button.onClick.AddListener(OnButtonClicked);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            _resultText.text = "Requested.";
            _hyperlinkButton.interactable = false;

            var sender = TezosManager.Instance.GetActiveAddress();
            const string entrypoint = "transfer";
            
            string input = "{ \"data\" : [ { \"prim\": \"Pair\", \"args\": [ { \"string\": \"" + sender + "\" }, [ { \"prim\": \"Pair\", \"args\": [ { \"string\": \"" + _inputFieldAddress.text + "\" }, { \"prim\": \"Pair\", \"args\": [ { \"int\": \"" + 0 + "\" }, { \"int\": \"" + _inputFieldAmount.text + "\" } ] } ] } ] ] } ] }";
            
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
