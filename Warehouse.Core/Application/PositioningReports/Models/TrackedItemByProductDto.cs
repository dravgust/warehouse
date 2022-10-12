namespace Warehouse.Core.Application.PositioningReports.Models
{
    public class TrackedItemByProductDto
    {
        public string Id { set; get; }
        public string Name { set; get; }
        public ICollection<SiteItem> Sites { set; get; }
    }

    public class SiteItem
    {
        public string Id { get; set; }
        public string Name { get; set; }
        public ICollection<BeaconItem> Beacons { set; get; }
    }

    public class SiteInfo
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
