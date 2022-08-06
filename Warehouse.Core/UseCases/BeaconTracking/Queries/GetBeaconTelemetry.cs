using MongoDB.Driver;
using Vayosoft.Core.Queries;
using Vayosoft.Data.MongoDB;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.BeaconTracking.Models;

namespace Warehouse.Core.UseCases.BeaconTracking.Queries
{
    public class GetBeaconTelemetry : IQuery<BeaconTelemetryDto>
    {
        public string MacAddress { set; get; }
    }

    public class GetBeaconTelemetry2 : IQuery<BeaconTelemetry2Dto>
    {
        public string MacAddress { set; get; }
    }

    public class HandleGetBeaconTelemetry :
        IQueryHandler<GetBeaconTelemetry, BeaconTelemetryDto>,
        IQueryHandler<GetBeaconTelemetry2, BeaconTelemetry2Dto>
    {
        private readonly IMongoConnection _connection;

        public HandleGetBeaconTelemetry(IMongoConnection connection)
        {
            _connection = connection;
        }

        public async Task<BeaconTelemetryDto> Handle(GetBeaconTelemetry request, CancellationToken cancellationToken)
        {
            var data = _connection.Collection<BeaconTelemetryEntity>()
                .AsQueryable()
                .Where(t => t.MacAddress == request.MacAddress)
                .OrderByDescending(m => m.ReceivedAt)
                .FirstOrDefault();
            if (data == null) return null;
            return await Task.FromResult(new BeaconTelemetryDto
            {
                MacAddress = data.MacAddress,
                ReceivedAt = data.ReceivedAt,
                Battery = data.Battery,
                Humidity = data.Humidity,
                RSSI = data.RSSI,
                Temperature = data.Temperature,
                TxPower = data.TxPower,
                X0 = data.X0,
                Y0 = data.Y0,
                Z0 = data.Z0
            });
        }

        public async Task<BeaconTelemetry2Dto> Handle(GetBeaconTelemetry2 request, CancellationToken cancellationToken)
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

            var result = new BeaconTelemetry2Dto
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
