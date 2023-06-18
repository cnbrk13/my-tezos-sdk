using Scripts.BeaconSDK;
using Scripts.Tezos.API;

namespace Tezos.Contracts
{
    public class FA2 : ContractBase
    {
        public FA2(IBeaconConnector beaconConnector, ITezosDataAPI api) : base(beaconConnector, api)
        {
        }
    }
}