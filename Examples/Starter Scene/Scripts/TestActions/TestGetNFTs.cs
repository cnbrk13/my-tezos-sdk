using Scripts.Helpers;
using Scripts.Tezos;
using TMPro;
using UnityEngine;
using Button = UnityEngine.UI.Button;

namespace Tezos.StarterScene
{
    public class TestGetNFTs : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Button _button;
        [SerializeField] private TextMeshProUGUI _contractAddressText;
        [SerializeField] private TextMeshProUGUI _resultText;
        [SerializeField] private RectTransform _trContent;
        [SerializeField] private GameObject _nftElementPrefab;
        
        private void Start()
        {
            _button.onClick.AddListener(OnGetNFTsButtonClicked);
        }

        private void OnDestroy()
        {
            _button.onClick.RemoveListener(OnGetNFTsButtonClicked);
        }
        
        private void OnGetNFTsButtonClicked()
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
                }
            }
        }
    }
}
