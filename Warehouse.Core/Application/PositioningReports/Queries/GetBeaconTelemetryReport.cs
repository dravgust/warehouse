using Vayosoft.Core.Queries;
using Warehouse.Core.Application.Common.Persistence;
using Warehouse.Core.Application.PositioningReports.Models;

namespace Warehouse.Core.Application.PositioningReports.Queries
{
    public class GetBeaconTelemetryReport : IQuery<BeaconTelemetryReport>
    {
        public GetBeaconTelemetryReport(string macAddress)
        {
            MacAddress = macAddress;
        }

        public string MacAddress { set; get; }
    }

    internal sealed class HandleGetBeaconTelemetryReport : IQueryHandler<GetBeaconTelemetryReport, BeaconTelemetryReport>
    {
        private readonly IWarehouseStore _store;

        public HandleGetBeaconTelemetryReport(IWarehouseStore store)
        {
            _store = store;
        }

        public async Task<BeaconTelemetryReport> Handle(GetBeaconTelemetryReport request, CancellationToken cancellationToken)
        {
            var data = await _store.GetBeaconTelemetryAsync(request.MacAddress, cancellationToken);
            var result = new BeaconTelemetryReport(request.MacAddress)
            {
                Humidity = new Dictionary<DateTime, double>(),
                Temperature = new Dictionary<DateTime, double>(),
            };

            foreach (var r in data)
            {
                if (r.Humidity != null)
                {
                    result.Humidity.Add(r.DateTime, Math.Round(r.Humidity.Value, 2));
                }
                if (r.Temperature != null)
                {
                    result.Temperature.Add(r.DateTime, Math.Round(r.Temperature.Value, 2));
                }
            }

            return result;
        }
    }
}
