namespace Warehouse.Core.UseCases.BeaconTracking.Models
{
    public class IndoorPositionStatusDto
    {
        public SiteInfo Site { set; get; }
        public ICollection<BeaconIndoorPositionInfo> In { set; get; }
        public ICollection<BeaconIndoorPositionInfo> Out { set; get; }
    }
    public class BeaconIndoorPositionInfo
    {
        public ProductInfo Product { set; get; }
        public BeaconInfo Beacon { set; get; }
    }
}
