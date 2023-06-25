using System;
using System.Linq;
using System.Text;
using System.Text.Json;
using Newtonsoft.Json.Linq;
using Scripts.Helpers;
using Scripts.Tezos;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace Tezos.StarterScene
{
    public class TestFA2GetNFTs : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _contractAddressText;
        [SerializeField] private TextMeshProUGUI _resultText;
        [SerializeField] private RectTransform _trContent;
        [SerializeField] private GameObject _nftElementPrefab;
        
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

            foreach (RectTransform tr in _trContent)
            {
                Destroy(tr.gameObject);
            }

            var activeWalletAddress = TezosManager.Instance.GetActiveAddress(); // Address to the current active account

            const string entrypoint = "view_items_of";
            var input = new { @string = activeWalletAddress };
            string contractAddress = _contractAddressText.text;
            CoroutineRunner.Instance.StartWrappedCoroutine(
                TezosManager.Instance.API.ReadView(
                    contractAddress: contractAddress,
                    entrypoint: entrypoint,
                    input: input,
                    callback: result =>
                    {
                        // Deserialize the json data to inventory items
                        CoroutineRunner.Instance.StartWrappedCoroutine(
                            NetezosExtensions.HumanizeValue(
                                val: result,
                                rpcUri: TezosConfig.Instance.RpcBaseUrl,
                                destination: contractAddress,
                                humanizeEntrypoint: "humanizeInventory",
                                onComplete: (NFTData[] inventory) =>
                                    OnInventoryFetched(inventory))
                        );
                        
                        _button.interactable = true;
                        _resultText.text = "Fetched.";
                    }));
        }
        
        private void OnInventoryFetched(NFTData[] inventory)
        {
            if (inventory != null)
            {
                foreach (var nftData in inventory)
                {
                    UINFTElement uinftElement = Instantiate(_nftElementPrefab, _trContent).GetComponent<UINFTElement>();
                    uinftElement.InitNFT(nftData);
                    if (nftData.id == "0")
                    {
                        const string entrypoint = "token_metadata";
                        var input = new {@int = nftData.id};
                        string contractAddress = _contractAddressText.text;
                        CoroutineRunner.Instance.StartWrappedCoroutine(
                            TezosManager.Instance.API.ReadView(
                                contractAddress: contractAddress,
                                entrypoint: entrypoint,
                                input: input,
                                callback: result =>
                                {
                                    JObject parsedJson = JObject.Parse(result.ToString());
                                    JArray elements = (JArray)parsedJson["args"][1];
        
                                    foreach (JObject element in elements)
                                    {
                                        if ((string)element["args"][0]["string"] == "artifactUri")
                                        {
                                            string artifactUriBytesHex = (string)element["args"][1]["bytes"];
                                            artifactUriBytesHex = artifactUriBytesHex.Substring(12); // Removing the prefix
                                            byte[] artifactUriBytes = Enumerable.Range(0, artifactUriBytesHex.Length)
                                                .Where(x => x % 2 == 0)
                                                .Select(x => Convert.ToByte(artifactUriBytesHex.Substring(x, 2), 16))
                                                .ToArray();
                
                                            string artifactUri = Encoding.ASCII.GetString(artifactUriBytes);
                                            uinftElement.InitNFTMetadata(artifactUri);
                                            Debug.Log("Token 0 Artifact Uri: " + artifactUri);
                                            break;
                                        }
                                    }
                                }));
                    }
                }
            }
        }
    }
}
