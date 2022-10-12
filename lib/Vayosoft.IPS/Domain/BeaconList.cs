using System.Collections.ObjectModel;

namespace Vayosoft.IPS.Domain
{
    public class BeaconList : KeyedCollection<string, IBeacon>
    {
        protected override string GetKeyForItem(IBeacon item) => item.MacAddress;
    }
}
