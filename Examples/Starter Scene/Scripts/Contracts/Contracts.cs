using Scripts.BeaconSDK;
using Scripts.Tezos.API;

namespace Tezos.Contracts
{
    public class Contracts
    {
        public FA2 FA2 { get; private set; }
        
        public Contracts(IBeaconConnector beaconConnector, ITezosDataAPI api)
        {
            FA2 = new FA2(beaconConnector, api);
        }
    }
}