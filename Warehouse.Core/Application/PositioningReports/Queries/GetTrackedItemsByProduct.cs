using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.Specifications;
using Warehouse.Core.Application.Common.Persistence;
using Warehouse.Core.Application.Common.Services;
using Warehouse.Core.Application.Common.Services.Security;
using Warehouse.Core.Application.PositioningReports.Models;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.Application.PositioningReports.Queries
{
    public record GetTrackedItemsByProduct : IQuery<IEnumerable<TrackedItemByProductDto>>;

    internal sealed class HandleGetDashboardByProduct : IQueryHandler<GetTrackedItemsByProduct, IEnumerable<TrackedItemByProductDto>>
    {
        private readonly IReadOnlyRepository<PositionStatus> _statuses;
        private readonly IReadOnlyRepository<WarehouseSiteEntity> _sites;
        private readonly IWarehouseStore _store;
        private readonly IReadOnlyRepository<ProductEntity> _products;
        private readonly IUserContext _userContext;

        public HandleGetDashboardByProduct(
            IReadOnlyRepository<PositionStatus> statuses,
            IReadOnlyRepository<WarehouseSiteEntity> sites,
            IWarehouseStore store,
            IReadOnlyRepository<ProductEntity> products,
            IUserContext userContext)
        {
            _statuses = statuses;
            _sites = sites;
            _store = store;
            _products = products;
            _userContext = userContext;
        }

        public async Task<IEnumerable<TrackedItemByProductDto>> Handle(GetTrackedItemsByProduct request, CancellationToken cancellationToken)
        {
            var result = new List<TrackedItemByProductDto>();

            var items = new Dictionary<(string, string), SiteItem>();
            var providerId = _userContext.User.Identity.GetProviderId();
            var spec = new Specification<WarehouseSiteEntity>(s => s.ProviderId == providerId);
            var sites = await _sites.ListAsync(spec, cancellationToken);
            foreach (var site in sites)
            {
                var status = await _statuses.FindAsync(site.Id, cancellationToken);
                if (status != null)
                {
                    foreach (var macAddress in status.In)
                    {
                        var beacon = await _store.TrackedItems
                            .FirstOrDefaultAsync(q => q.Id.Equals(macAddress), cancellationToken);
                        if (beacon != null && !string.IsNullOrEmpty(beacon.ProductId))
                        {
                            if (!items.TryGetValue((beacon.ProductId, site.Id), out var item))
                            {
                                item = new SiteItem
                                {
                                    Id = site.Id,
                                    Name = site.Name,
                                    Beacons = new List<BeaconItem>()
                                };
                                items.Add((beacon.ProductId, site.Id), item);
                            }

                            item.Beacons.Add(new BeaconItem
                            {
                                MacAddress = beacon.Id,
                                Name = beacon.Name
                            });
                        }
                    }
                }
            }


            foreach (var productGroup in items.GroupBy(s => s.Key.Item1))
            {
                var product = await _products.FirstOrDefaultAsync(p => p.Id == productGroup.Key, cancellationToken);
                if (product != null)
                {
                    var dashboardByProduct = new TrackedItemByProductDto
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Sites = new List<SiteItem>()
                    };
                    foreach (var groupValue in productGroup)
                    {
                        dashboardByProduct.Sites.Add(groupValue.Value);
                    }
                    
                    result.Add(dashboardByProduct);
                }
            }

            return result.OrderBy(s => s.Name);
        }
    }
}
