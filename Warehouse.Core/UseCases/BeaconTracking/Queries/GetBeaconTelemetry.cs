using MediatR;
using MongoDB.Driver;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.ValueObjects;
using Vayosoft.Core.Specifications;
using Vayosoft.Data.MongoDB;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.BeaconTracking.Models;

namespace Warehouse.Core.UseCases.BeaconTracking.Queries
{
    public class GetBeaconTelemetry : IQuery<BeaconTelemetryDto>, ILinqSpecification<BeaconTelemetryEntity>
    {
        public GetBeaconTelemetry(MacAddress macAddress)
        {
            MacAddress = macAddress;
        }

        public MacAddress MacAddress { get; }

        public IQueryable<BeaconTelemetryEntity> Apply(IQueryable<BeaconTelemetryEntity> query)
        {
            return query
                .Where(t => t.MacAddress == MacAddress)
                .OrderByDescending(m => m.ReceivedAt);
        }
    }

    public class HandleGetBeaconTelemetry : IQueryHandler<GetBeaconTelemetry, BeaconTelemetryDto>
    {
        private readonly IReadOnlyRepository<BeaconTelemetryEntity> _repository;

        public HandleGetBeaconTelemetry(IReadOnlyRepository<BeaconTelemetryEntity> repository)
        {
            _repository = repository;
        }

        public async Task<BeaconTelemetryDto> Handle(GetBeaconTelemetry request, CancellationToken cancellationToken)
        {
            var data = await _repository.FirstOrDefaultAsync(request, cancellationToken);
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
