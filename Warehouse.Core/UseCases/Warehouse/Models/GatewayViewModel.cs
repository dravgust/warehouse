using Warehouse.Core.Entities.Enums;

namespace Warehouse.Core.UseCases.Warehouse.Models
{
    public class GatewayViewModel
    {
        public string MacAddress { get; set; }
        public string Name { get; set; }
        public double CircumscribedRadius { get; set; }
        public LocationAnchor Location { get; set; }
        public int EnvFactor { set; get; }
        public BeaconViewModel Gauge { set; get; }
    }
}
