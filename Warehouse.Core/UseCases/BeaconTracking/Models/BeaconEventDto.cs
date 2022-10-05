using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.UseCases.BeaconTracking.Models
{
    public class BeaconEventDto
    { 
        public DateTime TimeStamp { get; set; }
        public BeaconItem Beacon { get; set; }
        public SiteInfo Source { get; set; }
        public SiteInfo Destination { get; set; }
        public BeaconEventType Type { set; get; }
    }
}
