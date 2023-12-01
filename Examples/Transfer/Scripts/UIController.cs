using System.Linq;
using TezosSDK.Tezos;
using TezosSDK.Tezos.API.Models.Filters;
using TMPro;
using UnityEngine;

namespace TezosSDK.Transfer.Scripts
{
    public class UIController : MonoBehaviour
    {
        [SerializeField] private GameObject transferControls;
        [SerializeField] private TextMeshProUGUI tokenIdsText;
        [SerializeField] private TextMeshProUGUI contractAddressText;

        private void Start()
        {
            transferControls.SetActive(false);

            var messageReceiver = TezosManager
                .Instance
                .MessageReceiver;

            messageReceiver.AccountConnected += _ =>
            {
                transferControls.SetActive(true);

                var contractAddress = TezosManager
                    .Instance
                    .Tezos
                    .TokenContract
                    .Address;

                if (!string.IsNullOrEmpty(contractAddress))
                {
                    GetContractTokenIds(contractAddress);
                    return;
                }

                var getOriginatedContractsRoutine = TezosManager
                    .Instance
                    .Tezos
                    .GetOriginatedContracts(contracts =>
                    {
                        var tokenContracts = contracts.ToList();
                        if (!tokenContracts.Any())
                        {
                            var activeAddress = TezosManager
                                .Instance
                                .Tezos
                                .Wallet
                                .GetActiveAddress();

                            tokenIdsText.text = $"{activeAddress} didn't deployed any contract yet.";
                            return;
                        }

                        var initializedContract = tokenContracts.First();
                        TezosManager
                            .Instance
                            .Tezos
                            .TokenContract = initializedContract;

                        contractAddressText.text = initializedContract.Address;
                        GetContractTokenIds(initializedContract.Address);
                    });

                StartCoroutine(getOriginatedContractsRoutine);
            };
            messageReceiver.AccountDisconnected += _ => { transferControls.SetActive(false); };
        }

        private void GetContractTokenIds(string contractAddress)
        {
            var tokensForContractCoroutine = TezosManager
                .Instance
                .Tezos
                .API
                .GetTokensForContract(
                    callback: tokens =>
                    {
                        var idsResult = tokens
                            .Aggregate(string.Empty, (resultString, token) => $"{resultString}{token.TokenId}, ");
                        tokenIdsText.text = idsResult[..^2];
                    },
                    contractAddress: contractAddress,
                    withMetadata: false,
                    maxItems: 10_000,
                    orderBy: new TokensForContractOrder.Default(0));

            StartCoroutine(tokensForContractCoroutine);
        }
    }
}