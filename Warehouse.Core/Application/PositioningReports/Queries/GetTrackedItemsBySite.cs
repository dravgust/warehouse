using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.Specifications;
using Warehouse.Core.Application.Common.Persistence;
using Warehouse.Core.Application.Common.Services;
using Warehouse.Core.Application.Common.Services.Security;
using Warehouse.Core.Application.PositioningReports.Models;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.Application.PositioningReports.Queries;

public record GetTrackedItemsBySite : IQuery<IEnumerable<TrackedItemBySiteDto>>
{ }

internal sealed class HandleGetDashboardBySite : IQueryHandler<GetTrackedItemsBySite, IEnumerable<TrackedItemBySiteDto>>
{
    private readonly IReadOnlyRepository<PositionStatus> _statuses;
    private readonly IReadOnlyRepository<WarehouseSiteEntity> _sites;
    private readonly IWarehouseStore _store;
    private readonly IReadOnlyRepository<ProductEntity> _products;
    private readonly IUserContext _userContext;

    public HandleGetDashboardBySite(
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

    public async Task<IEnumerable<TrackedItemBySiteDto>> Handle(GetTrackedItemsBySite request, CancellationToken cancellationToken)
    {
        var result = new List<TrackedItemBySiteDto>();

        var providerId = _userContext.User.Identity.GetProviderId();
        var spec = new Specification<WarehouseSiteEntity>(s => s.ProviderId == providerId);
        var sites = await _sites.ListAsync(spec, cancellationToken);
        foreach (var site in sites)
        {
            var dashboardBySite = new TrackedItemBySiteDto
            {
                Id = site.Id,
                Name = site.Name,
                Products = new List<ProductItem>(),
            };
            var status = await _statuses.FindAsync(site.Id, cancellationToken);
            if (status != null)
            {
                var items = new Dictionary<string, ProductItem>();
                foreach (var macAddress in status.In)
                {
                    var beacon = await _store.TrackedItems
                        .FirstOrDefaultAsync(q => q.Id.Equals(macAddress), cancellationToken);
                    if (beacon != null && !string.IsNullOrEmpty(beacon.ProductId))
                    {
                        var product = await _products
                            .FirstOrDefaultAsync(p => p.Id == beacon.ProductId, cancellationToken);
                        if (product != null)
                        {
                            if (!items.TryGetValue(beacon.ProductId, out var item))
                            {
                                item = new ProductItem
                                {
                                    Id = beacon.ProductId,
                                    Name = product.Name,
                                    Beacons = new List<BeaconItem>()
                                };
                                items.Add(beacon.ProductId, item);
                            }

                            item.Beacons.Add(new BeaconItem
                            {
                                MacAddress = beacon.Id,
                                Name = beacon.Name
                            });
                        }
                    }
                }

                foreach (var item in items)
                {
                    dashboardBySite.Products.Add(item.Value);
                }
            }

            result.Add(dashboardBySite);
        }
        return result.OrderBy(s => s.Name);
    }
}