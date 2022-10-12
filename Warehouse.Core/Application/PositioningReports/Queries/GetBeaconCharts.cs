using Vayosoft.Core.Queries;
using Warehouse.Core.Application.Common.Persistence;
using Warehouse.Core.Application.PositioningReports.Models;

namespace Warehouse.Core.Application.PositioningReports.Queries
{
    public class GetBeaconCharts : IQuery<BeaconCharts>
    {
        public GetBeaconCharts(string macAddress)
        {
            MacAddress = macAddress;
        }

        public string MacAddress { set; get; }
    }

    public class HandleGetBeaconCharts : IQueryHandler<GetBeaconCharts, BeaconCharts>
    {
        private readonly IWarehouseStore _store;

        public HandleGetBeaconCharts(IWarehouseStore store)
        {
            _store = store;
        }

        public async Task<BeaconCharts> Handle(GetBeaconCharts request, CancellationToken cancellationToken)
        {
            var data = await _store.GetBeaconTelemetryAsync(request.MacAddress, cancellationToken);
            var result = new BeaconCharts
            {
                MacAddress = request.MacAddress,
                Humidity = new Dictionary<DateTime, double>(),
                Temperature = new Dictionary<DateTime, double>(),
            };
            foreach (var r in data)
            {
                if (r.humidity != null)
                {
                    result.Humidity.Add(r._id, Math.Round(r.humidity.Value, 2));
                }
                if (r.temperatrue != null)
                {
                    result.Temperature.Add(r._id, Math.Round(r.temperatrue.Value, 2));
                }
            }

            return await Task.FromResult(result);
        }
    }
}
