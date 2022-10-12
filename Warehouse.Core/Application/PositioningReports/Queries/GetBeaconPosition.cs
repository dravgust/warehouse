using FluentValidation;
using System.Net.Mail;
using Vayosoft.Core.Caching;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.ValueObjects;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Application.Common.Services;
using Warehouse.Core.Application.Common.Services.Security;
using Warehouse.Core.Application.PositioningReports.Models;
using Warehouse.Core.Application.PositioningSystem.UseCases;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.Application.PositioningReports.Queries
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
        private readonly IQueryBus _queryBus;
        private readonly IReadOnlyRepository<IpsSettings> _settings;
        private readonly IUserContext _userContext;
        private readonly IDistributedMemoryCache _cache;

        public HandleGetBeaconPosition(
            IReadOnlyRepository<WarehouseSiteEntity> sites,
            IQueryBus queryBus,
            IUserContext userContext, IDistributedMemoryCache cache,
            IReadOnlyRepository<IpsSettings> settings)
        {
            _sites = sites;
            _queryBus = queryBus;
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

            var gSite = await _queryBus.Send(new GetGenericSite(site, settings), cancellationToken);
            gSite.CalcBeaconsPosition();

            return (from gw in gSite.Gateways
            from b in gw.Beacons
                    where b.MacAddress == request.MacAddress
                    select new BeaconPosition
                    {
                        GatewayId = gw.MacAddress,
                        MAC = b.MacAddress,
                        Radius = b.Radius,
                    }).ToList();
        }
    }
}