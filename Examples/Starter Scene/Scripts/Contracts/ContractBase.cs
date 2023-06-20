using Scripts.BeaconSDK;
using Scripts.Tezos.API;

namespace Tezos.Contracts
{
    public class ContractBase
    {
        protected IBeaconConnector BeaconConnector { get; private set; }
        protected ITezosDataAPI API { get; private set; }

        protected ContractBase(IBeaconConnector beaconConnector, ITezosDataAPI api)
        {
            BeaconConnector = beaconConnector;
            API = api;
        }
    }
}