using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tezos.StarterScene
{
    public class TestDeployContract : MonoBehaviour
    {
        [Header("References")]
        [SerializeField] private Button _deployButton;

        [SerializeField] private TextMeshProUGUI _textTxnHash;
        [SerializeField] private Button _hyperlinkButtonTxnHash;

        [SerializeField] private TextMeshProUGUI _textContractAddress;
        [SerializeField] private Button _hyperlinkButtonContractAddress;
        
        private void Start()
        {
            _deployButton.onClick.AddListener(OnButtonClicked);
        }

        private void OnDestroy()
        {
            _deployButton.onClick.RemoveListener(OnButtonClicked);
        }

        private void OnButtonClicked()
        {
            _deployButton.interactable = false;

            _textTxnHash.text = "";
            _hyperlinkButtonTxnHash.interactable = false;
            
            // TODO:
        }
    }
}