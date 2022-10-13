using Vayosoft.Core.Queries;
using Warehouse.Core.Application.Common.Persistence;
using Warehouse.Core.Application.PositioningReports.Models;

namespace Warehouse.Core.Application.PositioningReports.Queries
{
    public class GetBeaconCharts : IQuery<TelemetryViewModel>
    {
        public GetBeaconCharts(string macAddress)
        {
            MacAddress = macAddress;
        }

        public string MacAddress { set; get; }
    }

    public class HandleGetBeaconCharts : IQueryHandler<GetBeaconCharts, TelemetryViewModel>
    {
        private readonly IWarehouseStore _store;

        public HandleGetBeaconCharts(IWarehouseStore store)
        {
            _store = store;
        }

        public async Task<TelemetryViewModel> Handle(GetBeaconCharts request, CancellationToken cancellationToken)
        {
            var data = await _store.GetBeaconTelemetryAsync(request.MacAddress, cancellationToken);
            var result = new TelemetryViewModel
            {
                MacAddress = request.MacAddress,
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
