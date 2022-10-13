using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.ValueObjects;
using Warehouse.Core.Application.Common.Persistence;
using Warehouse.Core.Application.PositioningSystem.Domain.Entities;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.Application.PositioningSystem.UseCases.Queries
{
    public sealed record CreateGenericSite(WarehouseSiteEntity Site, IpsSettings Settings) : IQuery<GenericSite>;

    internal sealed class HandleGetGenericSite : IQueryHandler<CreateGenericSite, GenericSite>
    {
        private readonly IWarehouseStore _store;

        public HandleGetGenericSite(IWarehouseStore store)
        {
            _store = store;
        }

        public async Task<GenericSite> Handle(CreateGenericSite request, CancellationToken cancellationToken)
        {
            var gSite = new GenericSite(request.Site.Id)
            {
                TopLength = request.Site.TopLength,
                LeftLength = request.Site.LeftLength,
                Settings = request.Settings.GetCalculationSettings()
            };

            foreach (var gateway in request.Site.Gateways)
            {
                var gauge = gateway.Gauge;
                if (string.IsNullOrEmpty(gauge?.MAC)) continue;

                var payload = await _store.Payloads.FirstOrDefaultAsync(g => g.MacAddress == gateway.MacAddress, cancellationToken);
                if (payload is null) continue;

                var pGauge = payload.Beacons.FirstOrDefault(p => p.MacAddress.Equals(gauge.MAC, StringComparison.Ordinal));
                IBeacon beacon;
                if (pGauge is null)
                {
                    if (gauge.TxPower >= 0) continue;

                    beacon = new TelemetryBeacon(MacAddress.Empty, new List<double>(), gauge.TxPower, gauge.Radius);
                }
                else
                {
                    beacon = new TelemetryBeacon(gauge.MAC, pGauge.RSSIs, gauge.TxPower, gauge.Radius)
                    {
                        Battery = pGauge.Battery,
                        Temperature = pGauge.Temperature,
                        Humidity = pGauge.Humidity1,
                        X0 = pGauge.X0,
                        Y0 = pGauge.Y0,
                        Z0 = pGauge.Z0,
                    };
                }

                var gGateway = new GenericGateway(gateway.MacAddress)
                {
                    EnvFactor = gateway.EnvFactor,
                    Location = (LocationAnchor)gateway.Location,
                    Gauge = beacon
                };

                foreach (var b in payload.Beacons.Where(b => !b.MacAddress.Equals(gauge.MAC, StringComparison.Ordinal)))
                {
                    gGateway.AddBeacon(new TelemetryBeacon(b.MacAddress, b.RSSIs)
                    {
                        Battery = b.Battery,
                        Temperature = b.Temperature,
                        Humidity = b.Humidity1,
                        X0 = b.X0,
                        Y0 = b.Y0,
                        Z0 = b.Z0,
                    });
                }

                gSite.AddGateway(gGateway);
            }

            return gSite;
        }
    }
}
