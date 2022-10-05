using Warehouse.Core.Application.UseCases.SiteManagement.Models;

namespace Warehouse.Core.Application.UseCases.BeaconTracking.Models
{
    public class BeaconPosition : BeaconDto
    {
        public string GatewayId { get; set; }
    }
}
