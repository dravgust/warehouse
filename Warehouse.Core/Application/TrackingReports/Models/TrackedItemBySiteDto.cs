﻿namespace Warehouse.Core.Application.TrackingReports.Models
{
    public class TrackedItemBySiteDto
    {
        public string Id { set; get; }
        public string Name { set; get; }
        public ICollection<ProductItem> Products { set; get; }
    }

    public class ProductItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ICollection<BeaconItem> Beacons { set; get; }
    }

    public class BeaconItem
    {
        public string MacAddress { set; get; }
        public string Name { set; get; }
    }
}
