using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services;
using Warehouse.Core.UseCases.BeaconTracking.Models;
using Warehouse.Core.Utilities;

namespace Warehouse.Core.UseCases.BeaconTracking.Queries;

public class GetDashboardBySite : IQuery<IEnumerable<DashboardBySite>>
{ }

public class HandleGetDashboardBySite : IQueryHandler<GetDashboardBySite, IEnumerable<DashboardBySite>>
{
    private readonly IReadOnlyRepository<IndoorPositionStatusEntity> _statuses;
    private readonly IReadOnlyRepository<WarehouseSiteEntity> _sites;
    private readonly IReadOnlyRepository<BeaconEntity> _beacons;
    private readonly IReadOnlyRepository<ProductEntity> _products;
    private readonly IUserContext _userContext;

    public HandleGetDashboardBySite(
        IReadOnlyRepository<IndoorPositionStatusEntity> statuses,
        IReadOnlyRepository<WarehouseSiteEntity> sites,
        IReadOnlyRepository<BeaconEntity> beacons,
        IReadOnlyRepository<ProductEntity> products,
        IUserContext userContext)
    {
        _statuses = statuses;
        _sites = sites;
        _beacons = beacons;
        _products = products;
        _userContext = userContext;
    }

    public async Task<IEnumerable<DashboardBySite>> Handle(GetDashboardBySite request, CancellationToken cancellationToken)
    {
        var result = new List<DashboardBySite>();

        var providerId = _userContext.User.Identity.GetProviderId();
        var sites = await _sites.ListAsync(s => s.ProviderId == providerId, cancellationToken);
        foreach (var site in sites)
        {
            var status = await _statuses.FindAsync(site.Id, cancellationToken);
            if (status != null)
            {
                var items = new Dictionary<string, ProductItem>();
                foreach (var macAddress in status.In)
                {
                    var beacon = await _beacons
                        .FirstOrDefaultAsync(q => q.Id.Equals(macAddress), cancellationToken);
                    if (beacon != null && !string.IsNullOrEmpty(beacon.ProductId))
                    {
                        var product = await _products
                            .FirstOrDefaultAsync(p => p.Id == beacon.ProductId, cancellationToken);
                        if (product != null)
                        {
                            if (!items.TryGetValue(product.Id, out var item))
                            {
                                item = new ProductItem
                                {
                                    Id = product.Id,
                                    Name = product.Name,
                                    Beacons = new List<BeaconItem>()
                                };
                                items.Add(product.Id, item);
                            }

                            item.Beacons.Add(new BeaconItem
                            {
                                MacAddress = beacon.MacAddress,
                                Name = beacon.Name
                            });
                        }
                    }
                }

                var dashboardBySite = new DashboardBySite
                {
                    Id = site.Id,
                    Name = site.Name,
                    Products = new List<ProductItem>(),
                };
                foreach (var item in items)
                {
                    dashboardBySite.Products.Add(item.Value);
                }
                result.Add(dashboardBySite);
            }
            
        }
        return result;
    }
}