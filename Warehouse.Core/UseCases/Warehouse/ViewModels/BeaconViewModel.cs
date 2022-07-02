using Warehouse.Core.Domain.Enums;

namespace Warehouse.Core.UseCases.Warehouse.ViewModels
{
    public class BeaconViewModel
    {
        public string MAC { set; get; }
        public double TxPower { set; get; }
        public double RSSI { set; get; }
        public double Radius { set; get; }
        public List<double> RSSIs { set; get; }
        public List<double> OriginalRSSIs { set; get; }
        public bool IsGage { set; get; } = false;
        public LocationAnchor Location { set; get; } = LocationAnchor.Unknown;
    }
}
