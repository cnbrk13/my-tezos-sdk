using System;
using System.Collections;
using System.Collections.Generic;
using System.Text.Json;
using Netezos.Encoding;
using Scripts.BeaconSDK;
using Scripts.Tezos;
using Scripts.Tezos.API;

namespace Tezos.Contracts
{
    public class FA12 : ContractBase
    {
        public FA12(IBeaconConnector beaconConnector, ITezosDataAPI api) : base(beaconConnector, api)
        {
        }
        
        public void Transfer(string contractAddress, string from, string to, ulong amount = 0)
        {
            const string entryPoint = "transfer";

            var parameter = new MichelinePrim
            {
                Prim = PrimType.Pair,
                Args = new List<IMicheline>
                {
                    new MichelineString(from),
                    new MichelineString(to),
                    new MichelineInt(amount)
                }
            }.ToJson();

            BeaconConnector.RequestTezosOperation(
                destination: contractAddress,
                entryPoint: entryPoint,
                arg: parameter,
                amount: 0,
                networkName: TezosConfig.Instance.Network.ToString(),
                networkRPC: TezosConfig.Instance.RpcBaseUrl);
        }

        public IEnumerator GetBalance(string contractAddress, string address, Action<JsonElement> callback)
        {
            const string entryPoint = "getBalance";
            var parameter = new MichelinePrim
            {
                Prim = PrimType.Pair,
                Args = new List<IMicheline>
                {
                    new MichelineString(address),
                }
            }.ToJson();
            return API.ReadView(contractAddress, entryPoint, parameter, callback);
        }

        public IEnumerator GetTotalSupply(string contractAddress, Action<JsonElement> callback)
        {
            const string entryPoint = "getTotalSupply";
            var parameter = new MichelinePrim
            {
                Prim = PrimType.Pair,
                Args = new List<IMicheline>
                {

                }
            }.ToJson();
            return API.ReadView(contractAddress, entryPoint, parameter, callback);
        }
    }
}