namespace Warehouse.Core.UseCases.BeaconTracking.Models
{
    public class DashboardBySite
    {
        public SiteInfo Site { set; get; }
        public ICollection<DashboardBySiteItem> In { set; get; }
        public ICollection<DashboardBySiteItem> Out { set; get; }
    }
    public class DashboardBySiteItem
    {
        public ProductInfo Product { set; get; }
        public BeaconInfo Beacon { set; get; }
    }
}
