using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tezos.StarterScene
{
    public struct NFTData
    {
        public string id { get; set; }
        public string amount { get; set; }
        public ContractItem item { get; set; }
    }

    public struct NFTMetadata
    {
        public string token_id { get; set; }
        public TokenInfo token_info { get; set; }
    }
    
    public struct TokenInfo
    {
        public string artifactUri { get; set; }
    
    }
    
    public struct ContractItem
    {
        public string damage { get; set; }
        public string armor { get; set; }
        public string attackSpeed { get; set; }
        public string healthPoints { get; set; }
        public string manaPoints { get; set; }
        public string itemType { get; set; }
    }
    
    public class UINFTElement : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Image _nftImage;
        [SerializeField] private TextMeshProUGUI _idText;
        [SerializeField] private TextMeshProUGUI _amountText;
        
        public void InitNFT(NFTData nftData)
        {
            _idText.text = "ID: " + nftData.id;
            _amountText.text = "Amount: " + nftData.amount;
        }

        public void InitNFTMetadata(string uri)
        {
            //StartCoroutine(IPFSImageDownloader.DownloadAndSetImageTexture(uri, _nftImage));
        }
    }
}