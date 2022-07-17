using Vayosoft.Core.Queries;
using Warehouse.Core.UseCases.IPS.Models;

namespace Warehouse.Core.UseCases.IPS.Queries
{
    public class GetBeaconPayload : IQuery<BeaconTelemetryDto>
    {
        public string MacAddress { set; get; }
    }
}
