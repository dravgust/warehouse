using Warehouse.Core.Entities.Enums;

namespace Warehouse.Core.UseCases.Warehouse.Models
{
    public class GatewayDto
    {
        public string MacAddress { get; set; }
        public string Name { get; set; }
        public double CircumscribedRadius { get; set; }
        public LocationAnchor Location { get; set; }
        public int EnvFactor { set; get; }
        public BeaconDto Gauge { set; get; }
    }
}
