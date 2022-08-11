using MongoDB.Driver;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.ValueObjects;
using Vayosoft.Data.MongoDB;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.BeaconTracking.Models;

namespace Warehouse.Core.UseCases.BeaconTracking.Queries
{
    public class GetBeaconTelemetry : IQuery<BeaconTelemetryDto>
    {
        public GetBeaconTelemetry(MacAddress macAddress)
        {
            MacAddress = macAddress;
        }

        public MacAddress MacAddress { get; }
    }

    public class HandleGetBeaconTelemetry : IQueryHandler<GetBeaconTelemetry, BeaconTelemetryDto>
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
    }
}
