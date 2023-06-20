using Scripts.BeaconSDK;
using Scripts.Tezos;
using Scripts.Tezos.API;

namespace Tezos.Contracts
{
    /// <summary>
    /// Interact with any FA2 compatible contract.
    /// </summary>
    public interface IFA2
    {
        void Mint(string contractAddress);
    }
    
    public class FA2 : ContractBase, IFA2
    {
        public FA2(IBeaconConnector beaconConnector, ITezosDataAPI api) : base(beaconConnector, api)
        {
        }
        
        public void Mint(string contractAddress)
        {
            const string entryPoint = "mint";
            
            string input = "{\"prim\": \"Unit\"}";

            BeaconConnector.RequestTezosOperation(
                destination: contractAddress,
                entryPoint: entryPoint,
                arg: input,
                amount: 0,
                networkName: TezosConfig.Instance.Network.ToString(),
                networkRPC: TezosConfig.Instance.RpcBaseUrl);
        }
    }
}