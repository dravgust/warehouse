using Warehouse.Core.Entities.Models;

namespace Warehouse.Core.UseCases.BeaconTracking.Models
{
    public class BeaconEventDto
    { 
        public DateTime TimeStamp { get; set; }
        public BeaconInfo Beacon { get; set; }
        public SiteInfo Source { get; set; }
        public SiteInfo Destination { get; set; }
        public BeaconEventType Type { set; get; }
    }
}
