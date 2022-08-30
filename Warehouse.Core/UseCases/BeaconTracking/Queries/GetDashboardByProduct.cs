using System.Linq;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services;
using Warehouse.Core.UseCases.BeaconTracking.Models;
using Warehouse.Core.Utilities;

namespace Warehouse.Core.UseCases.BeaconTracking.Queries
{
    public class GetDashboardByProduct : IQuery<IEnumerable<DashboardByProduct>>
    { }

    internal class HandleGetDashboardByProduct : IQueryHandler<GetDashboardByProduct, IEnumerable<DashboardByProduct>>
    {
        private readonly IReadOnlyRepository<IndoorPositionStatusEntity> _statuses;
        private readonly IReadOnlyRepository<WarehouseSiteEntity> _sites;
        private readonly IReadOnlyRepository<BeaconEntity> _beacons;
        private readonly IReadOnlyRepository<ProductEntity> _products;
        private readonly IUserContext _userContext;

        public HandleGetDashboardByProduct(
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

        public async Task<IEnumerable<DashboardByProduct>> Handle(GetDashboardByProduct request, CancellationToken cancellationToken)
        {
            var result = new List<DashboardByProduct>();

            var items = new Dictionary<(string, string), SiteItem>();
            var providerId = _userContext.User.Identity.GetProviderId();
            var sites = await _sites.ListAsync(s => s.ProviderId == providerId, cancellationToken);
            foreach (var site in sites)
            {
                var status = await _statuses.FindAsync(site.Id, cancellationToken);
                if (status != null)
                {
                    foreach (var macAddress in status.In)
                    {
                        var beacon = await _beacons
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
                                MacAddress = beacon.MacAddress,
                                Name = beacon.Name
                            });
                        }
                    }
                }
            }


            foreach (var siteItemGroup in items.GroupBy(s => s.Key.Item1))
            {
                var product = await _products.FirstOrDefaultAsync(p => p.Id == siteItemGroup.Key, cancellationToken);
                if (product != null)
                {
                    var dashboardByProduct = new DashboardByProduct
                    {
                        Id = product.Id,
                        Name = product.Name,
                        Sites = new List<SiteItem>()
                    };
                    foreach (var groupValue in siteItemGroup)
                    {
                        dashboardByProduct.Sites.Add(groupValue.Value);
                    }
                    
                    result.Add(dashboardByProduct);
                }
            }

            return result;
        }
    }
}
