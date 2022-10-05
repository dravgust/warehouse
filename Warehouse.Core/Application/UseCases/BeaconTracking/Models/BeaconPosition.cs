using Warehouse.Core.Application.UseCases.Management.Models;

namespace Warehouse.Core.Application.UseCases.BeaconTracking.Models
{
    public class BeaconPosition : BeaconDto
    {
        public string GatewayId { get; set; }
    }
}
