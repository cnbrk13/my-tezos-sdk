using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using Netezos.Contracts;
using Netezos.Encoding;
using Scripts.BeaconSDK;
using Scripts.Tezos;
using Scripts.Tezos.API;
using UnityEngine;

namespace Tezos.Contracts
{
    /// <summary>
    /// Interact with any FA2 compatible contract.
    /// </summary>
    public interface IFA2
    {
        void Mint(string contractAddress, string address, uint tokenId, uint amount);
        void Transfer(string contractAddress, string fromAddress, string toAddress, decimal amount);
        void UpdateOperators(string contractAddress, string operatorAddress, string owner, uint tokenId, bool add);
        IEnumerator BalanceOf(string contractAddress, string owner, uint tokenId, Action<JsonElement> callback);
    }

    
    public class FA2 : ContractBase, IFA2
    {
        public FA2(IBeaconConnector beaconConnector, ITezosDataAPI api) : base(beaconConnector, api)
        {
        }
    
        public void Mint(string contractAddress, string address, uint tokenId, uint amount)
        {
            const string entryPoint = "mint";
            //string input = "{\"prim\": \"Unit\"}";
            var input = new MichelinePrim
            {
                Prim = PrimType.Pair,
                Args = new List<IMicheline>
                {
                    new MichelineString(address),
                    new MichelineInt(amount),
                    null
                }
            }.ToJson();
            
            BeaconConnector.RequestTezosOperation(
                destination: contractAddress,
                entryPoint: entryPoint,
                arg: input,
                amount: 0,
                networkName: TezosConfig.Instance.Network.ToString(),
                networkRPC: TezosConfig.Instance.RpcBaseUrl);
        }
    
        public void Transfer(string contractAddress, string fromAddress, string toAddress, decimal amount)
        {
            const string entryPoint = "transfer";
            string input = $"{{\"prim\": \"Pair\", \"args\": [{{\"string\": \"{fromAddress}\"}}, {{\"string\": \"{toAddress}\"}}, {amount}]}}";
    
            BeaconConnector.RequestTezosOperation(
                destination: contractAddress,
                entryPoint: entryPoint,
                arg: input,
                amount: 0,
                networkName: TezosConfig.Instance.Network.ToString(),
                networkRPC: TezosConfig.Instance.RpcBaseUrl);
        }
        
        public void UpdateOperators(string contractAddress, string operatorAddress, string owner, uint tokenId, bool add)
        {
            const string entryPoint = "update_operators";
            string action = add ? "add_operator" : "remove_operator";

            string input = $"{{\"prim\": \"Pair\", \"args\": [{{\"string\": \"{action}\"}}, {{\"prim\": \"Pair\", \"args\": [{{\"string\": \"{operatorAddress}\"}}, {{\"prim\": \"Pair\", \"args\": [{{\"string\": \"{owner}\"}}, {{\"int\": \"{tokenId}\"}}]}}]}}]}}";

            BeaconConnector.RequestTezosOperation(
                destination: contractAddress,
                entryPoint: entryPoint,
                arg: input,
                amount: 0,
                networkName: TezosConfig.Instance.Network.ToString(),
                networkRPC: TezosConfig.Instance.RpcBaseUrl);
        }
        
        public IEnumerator BalanceOf(string contractAddress, string owner, uint tokenId, Action<JsonElement> callback)
        {
            const string entryPoint = "balance_of";
            var input = new {@string = owner, @uint = tokenId};
    
            yield return TezosManager.Instance.API.ReadView(
                contractAddress: contractAddress,
                entrypoint: entryPoint,
                input: input,
                callback: callback
            );
        }
    }
}