using System.Collections.Generic;
using Scripts.Tezos.Wallet;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Tezos.StarterScene
{
    public class StarterUIManager : MonoBehaviour
    {
        [SerializeField] private GameObject _connectPanel;
        [SerializeField] private GameObject _qrCodePanel;
        [SerializeField] private GameObject _testingPanel;
        [SerializeField] private List<GameObject> _tabPanels;
        [SerializeField] private List<Toggle> _panelToggles;
        [SerializeField] private TextMeshProUGUI _accountAddressText;

        private void Start()
        {
            _connectPanel.SetActive(true);
            _testingPanel.SetActive(false);
            _qrCodePanel.SetActive(false);
            TezosManager.Instance.MessageReceiver.AccountConnected += OnAccountConnected;
            TezosManager.Instance.MessageReceiver.AccountDisconnected += OnAccountDisconnected;
        }

        private void OnDestroy()
        {
            TezosManager.Instance.MessageReceiver.AccountConnected -= OnAccountConnected;
            TezosManager.Instance.MessageReceiver.AccountDisconnected -= OnAccountDisconnected;
        }

        private void OnAccountConnected(string accountInfo)
        {
            _connectPanel.SetActive(false);
            _qrCodePanel.SetActive(false);
            _testingPanel.SetActive(true);
            
            _accountAddressText.text = TezosManager.Instance.GetActiveAddress();
            for (int i = 0; i < _tabPanels.Count; i++)
            {
                _tabPanels[i].SetActive(i == 0);
            }

            for (int i = 0; i < _panelToggles.Count; i++)
            {
                _panelToggles[i].isOn = i == 0;
            }
        }

        private void OnAccountDisconnected(string accountInfo)
        {
            _connectPanel.SetActive(true);
            _qrCodePanel.SetActive(false);
            _testingPanel.SetActive(false);
        }
        
        public void OnNativeConnectButtonClicked()
        {
            TezosManager.Instance.Connect(WalletProviderType.beacon, true);
        }

        public void OnDisconnectButtonClicked()
        {
            TezosManager.Instance.Disconnect();
        }
    }
}