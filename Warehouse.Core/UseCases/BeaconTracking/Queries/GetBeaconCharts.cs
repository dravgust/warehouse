using MongoDB.Driver;
using Vayosoft.Core.Queries;
using Vayosoft.Data.MongoDB;
using Warehouse.Core.Domain.Entities;
using Warehouse.Core.UseCases.BeaconTracking.Models;

namespace Warehouse.Core.UseCases.BeaconTracking.Queries
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
        private readonly IMongoConnection _connection;

        public HandleGetBeaconCharts(IMongoConnection connection)
        {
            _connection = connection;
        }

        public async Task<BeaconCharts> Handle(GetBeaconCharts request, CancellationToken cancellationToken)
        {
            var data = _connection.Collection<BeaconTelemetryEntity>().Aggregate()
                .Match(t => t.MacAddress == request.MacAddress && t.ReceivedAt > DateTime.UtcNow.AddHours(-12))
                .Group(k =>
                        new DateTime(k.ReceivedAt.Year, k.ReceivedAt.Month, k.ReceivedAt.Day,
                            k.ReceivedAt.Hour - (k.ReceivedAt.Hour % 1), 0, 0),
                    g => new
                    {
                        _id = g.Key,
                        humidity = g.Where(entity => entity.Humidity > 0).Average(entity => entity.Humidity),
                        temperatrue = g.Where(entity => entity.Temperature > 0).Average(entity => entity.Temperature)
                    }
                )
                .SortBy(d => d._id)
                .ToList();

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
