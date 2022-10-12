using System.Collections.ObjectModel;

namespace Warehouse.Core.Application.PositioningSystem.Domain
{
    public class BeaconList : KeyedCollection<string, IBeacon>
    {
        protected override string GetKeyForItem(IBeacon item) => item.MacAddress;
    }
}
