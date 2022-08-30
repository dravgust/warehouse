using Warehouse.Core.UseCases.Management.Models;

namespace Warehouse.Core.UseCases.BeaconTracking.Models
{
    public class BeaconPosition : BeaconDto
    {
        public string GatewayId { get; set; }
    }
}
