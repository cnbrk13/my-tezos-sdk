using Scripts.BeaconSDK;
using Scripts.Tezos.API;

namespace Tezos.Contracts
{
    public class ContractBase
    {
        public IBeaconConnector BeaconConnector { get; private set; }
        public ITezosDataAPI API { get; private set; }

        public ContractBase(IBeaconConnector beaconConnector, ITezosDataAPI api)
        {
            BeaconConnector = beaconConnector;
            API = api;
        }
    }
}