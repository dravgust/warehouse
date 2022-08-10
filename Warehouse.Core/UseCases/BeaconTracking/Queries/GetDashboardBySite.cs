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
        var items = new Dictionary<string, DashboardBySiteItem>();
        foreach (var site in sites)
        {
            var dashboardBySite = new DashboardBySite
            {
                Site = new SiteInfo
                {
                    Id = site.Id,
                    Name = site.Name
                },
                In = new List<DashboardBySiteItem>(),
                Out = new List<DashboardBySiteItem>()
            };
            var status = await _statuses.FindAsync(site.Id, cancellationToken);
            if (status != null)
            {
                foreach (var macAddress in status.In)
                {
                    dashboardBySite.In.Add(await GetItem(macAddress, items, cancellationToken));
                }
                foreach (var macAddress in status.Out)
                {
                    dashboardBySite.Out.Add(await GetItem(macAddress, items, cancellationToken));
                }
            }
            result.Add(dashboardBySite);
        }
        return result;
    }

    private async Task<DashboardBySiteItem> GetItem(string macAddress,
        IDictionary<string, DashboardBySiteItem> items, CancellationToken cancellationToken)
    {
        if (items.TryGetValue(macAddress, out var item)) return item;
        item = new DashboardBySiteItem
        {
            Beacon = new BeaconInfo
            {
                MacAddress = macAddress
            },
            Product = new ProductInfo
            {
                Id = string.Empty
            }
        };
        var beacon = await _beacons
            .FirstOrDefaultAsync(q => q.Id.Equals(macAddress), cancellationToken);
        if (beacon != null)
        {
            item.Beacon.Name = beacon.Name;

            if (!string.IsNullOrEmpty(beacon.ProductId))
            {
                var product = await _products
                    .FirstOrDefaultAsync(p => p.Id == beacon.ProductId, cancellationToken);
                if (product != null)
                {
                    item.Product.Id = product.Id;
                    item.Product.Name = product.Name;
                }
            }
        }
        items.Add(macAddress, item);
        return item;
    }
}