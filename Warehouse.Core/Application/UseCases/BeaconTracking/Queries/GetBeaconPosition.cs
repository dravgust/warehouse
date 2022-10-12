using FluentValidation;
using Vayosoft.Core.Caching;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.ValueObjects;
using Vayosoft.Core.Utilities;
using Vayosoft.IPS.Domain;
using Warehouse.Core.Application.Services;
using Warehouse.Core.Application.Services.Security;
using Warehouse.Core.Application.UseCases.BeaconTracking.Models;
using Warehouse.Core.Domain.Entities;
using Warehouse.Core.Domain.Entities.Payloads;

namespace Warehouse.Core.Application.UseCases.BeaconTracking.Queries
{
    public class GetBeaconPosition : IQuery<ICollection<BeaconPosition>>
    {
        public GetBeaconPosition(string siteId, MacAddress macAddress)
        {
            SiteId = siteId;
            MacAddress = macAddress;
        }

        public MacAddress MacAddress { get; }
        public string SiteId { get; }

        public class AlertRequestValidator : AbstractValidator<GetBeaconPosition>
        {
            public AlertRequestValidator()
            {
                RuleFor(q => q.SiteId).NotEmpty();
            }
        }
    }

    public class HandleGetBeaconPosition : IQueryHandler<GetBeaconPosition, ICollection<BeaconPosition>>
    {
        private readonly IReadOnlyRepository<WarehouseSiteEntity> _sites;
        private readonly IReadOnlyRepository<GatewayPayload> _payloads;
        private readonly IReadOnlyRepository<IpsSettings> _settings;
        private readonly IUserContext _userContext;
        private readonly IDistributedMemoryCache _cache;

        public HandleGetBeaconPosition(
            IReadOnlyRepository<WarehouseSiteEntity> sites,
            IReadOnlyRepository<GatewayPayload> payloads,
            IUserContext userContext, IDistributedMemoryCache cache,
            IReadOnlyRepository<IpsSettings> settings)
        {
            _sites = sites;
            _payloads = payloads;
            _userContext = userContext;
            _cache = cache;
            _settings = settings;
        }

        public async Task<ICollection<BeaconPosition>> Handle(GetBeaconPosition request, CancellationToken cancellationToken)
        {
            var providerId = _userContext.User.Identity.GetProviderId();
            var site = await _sites.FirstOrDefaultAsync(s => s.Id == request.SiteId && s.ProviderId == providerId,
                cancellationToken);
            if (site?.Gateways == null) return null;

            var settings = await _cache.GetOrCreateExclusiveAsync(CacheKey.With<IpsSettings>(), async options =>
            {
                options.AbsoluteExpirationRelativeToNow = TimeSpans.FiveMinutes;
                return await _settings.FirstOrDefaultAsync(e => true, cancellationToken: cancellationToken) ??
                       new IpsSettings();
            });

            var gSite = await GetGenericSiteAsync(request.MacAddress, _payloads, site, settings);
            gSite.CalcBeaconsPosition();

            return (from gw in gSite.Gateways
                    from b in gw.Beacons
                    select new BeaconPosition
                    {
                        GatewayId = gw.MacAddress,
                        MAC = b.MacAddress,
                        Radius = b.Radius,
                    }).ToList();
        }

        private static async Task<GenericSite> GetGenericSiteAsync(MacAddress macAddress,
            IReadOnlyRepository<GatewayPayload> repository, WarehouseSiteEntity site, IpsSettings settings)
        {
            var gSite = new GenericSite(site.Id)
            {
                TopLength = site.TopLength,
                LeftLength = site.LeftLength,
                Settings = settings.GetCalculationSettings()
            };

            foreach (var gateway in site.Gateways)
            {
                var gauge = gateway.Gauge;
                if (string.IsNullOrEmpty(gauge?.MAC)) continue;

                var payload = await repository.FirstOrDefaultAsync(g => g.MacAddress == gateway.MacAddress);
                if (payload is null) continue;

                var pGauge =
                    payload.Beacons.FirstOrDefault(p => p.MacAddress.Equals(gauge.MAC, StringComparison.Ordinal));
                IBeacon beacon;
                if (pGauge is null)
                {
                    if(gauge.TxPower >= 0) continue;

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
                    Location = (LocationAnchor) gateway.Location,
                    Gauge = beacon
                };

                var b = payload.Beacons.FirstOrDefault(b => b.MacAddress.Equals(macAddress, StringComparison.Ordinal));
                if (b is not null)
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