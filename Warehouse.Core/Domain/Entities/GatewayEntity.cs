
using Warehouse.Core.Domain.Enums;

namespace Warehouse.Core.Domain.Entities
{
    public class Gateway
    {
        public string MacAddress { get; set; }
        public string Name { get; set; }
        public double CircumscribedRadius { get; set; }
        public LocationAnchor Location { get; set; }
        public int EnvFactor { set; get; }
        public Beacon Gauge { set; get; }
    }
}
