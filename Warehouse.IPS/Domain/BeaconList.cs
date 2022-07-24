﻿using System.Collections.ObjectModel;

namespace Warehouse.IPS.Domain
{
    public class BeaconList : KeyedCollection<string, IBeacon>
    {
        protected override string GetKeyForItem(IBeacon item) => item.MacAddress;
    }
}
