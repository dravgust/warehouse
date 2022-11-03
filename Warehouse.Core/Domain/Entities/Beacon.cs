using Vayosoft.Commons.Entities;
using Warehouse.Core.Domain.Enums;

namespace Warehouse.Core.Domain.Entities
{
    public class Beacon : IEntity<string>
    {
        public string MAC { set; get; }
        public double TxPower { set; get; }
        public double RSSI { set; get; }
        public double Radius { set; get; }
        public List<double> RSSIs { set; get; }
        public List<double> OriginalRSSIs { set; get; }
        public bool IsGage { set; get; } = false;
        public LocationAnchor Location { set; get; } = LocationAnchor.Unknown;
        object IEntity.Id => Id;

        public string Id
        {
            set => MAC = value;
            get => MAC;
        }
    }
}
