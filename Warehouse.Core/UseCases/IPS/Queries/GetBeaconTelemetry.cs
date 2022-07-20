using Vayosoft.Core.Queries;
using Warehouse.Core.UseCases.IPS.Models;

namespace Warehouse.Core.UseCases.IPS.Queries
{
    public class GetBeaconTelemetry : IQuery<BeaconTelemetryDto>
    {
        public string MacAddress { set; get; }
    }

    public class GetBeaconTelemetry2 : IQuery<BeaconTelemetry2Dto>
    {
        public string MacAddress { set; get; }
    }
}
