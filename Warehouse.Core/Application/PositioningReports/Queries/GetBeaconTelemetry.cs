﻿using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.ValueObjects;
using Vayosoft.Core.Specifications;
using Warehouse.Core.Application.PositioningReports.Models;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.Application.PositioningReports.Queries
{
    public sealed class GetBeaconTelemetry : IQuery<BeaconTelemetryDto>, ILinqSpecification<BeaconTelemetry>
    {
        public GetBeaconTelemetry(MacAddress macAddress)
        {
            MacAddress = macAddress;
        }

        public MacAddress MacAddress { get; }

        public IQueryable<BeaconTelemetry> Apply(IQueryable<BeaconTelemetry> query)
        {
            return query
                .Where(t => t.MacAddress == MacAddress)
                .OrderByDescending(m => m.ReceivedAt);
        }
    }

    internal sealed class HandleGetBeaconTelemetry : IQueryHandler<GetBeaconTelemetry, BeaconTelemetryDto>
    {
        private readonly IReadOnlyRepository<BeaconTelemetry> _repository;

        public HandleGetBeaconTelemetry(IReadOnlyRepository<BeaconTelemetry> repository)
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
