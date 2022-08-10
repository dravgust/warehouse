using System.Collections.ObjectModel;
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
        private readonly IReadOnlyRepository<BeaconReceivedEntity> _receivedBeacons;
        private readonly IReadOnlyRepository<WarehouseSiteEntity> _sites;
        private readonly IReadOnlyRepository<BeaconEntity> _beacons;
        private readonly IReadOnlyRepository<ProductEntity> _products;
        private readonly IUserContext _userContext;

        public HandleGetDashboardByProduct(
            IReadOnlyRepository<BeaconReceivedEntity> receivedBeacons,
            IReadOnlyRepository<WarehouseSiteEntity> sites,
            IReadOnlyRepository<BeaconEntity> beacons,
            IReadOnlyRepository<ProductEntity> products,
            IUserContext userContext)
        {
            _receivedBeacons = receivedBeacons;
            _sites = sites;
            _beacons = beacons;
            _products = products;
            _userContext = userContext;
        }

        public async Task<IEnumerable<DashboardByProduct>> Handle(GetDashboardByProduct request, CancellationToken cancellationToken)
        {
            var result = new List<DashboardByProduct>();

            var providerId = _userContext.User.Identity.GetProviderId();
            var sites = new Dictionary<string, SiteInfo>();
            var items = new Dictionary<string, List<DashboardByProductItem>>();
            var beacons = await _beacons.ListAsync(b => b.ProductId != null && b.ProviderId == providerId, cancellationToken);
            foreach (var beacon in beacons)
            {
                var item = new DashboardByProductItem
                {
                    Beacon = new BeaconInfo
                    {
                        MacAddress = beacon.MacAddress,
                        Name = beacon.Name
                    },
                };

                SiteInfo siteInfo = null;
                var receivedBeacon = await _receivedBeacons.FindAsync(beacon.MacAddress, cancellationToken);
                if (receivedBeacon != null)
                {
                    if (!string.IsNullOrEmpty(receivedBeacon.SourceId))
                    {
                        if (!sites.TryGetValue(receivedBeacon.SourceId, out siteInfo))
                        {
                            siteInfo = new SiteInfo
                            {
                                Id = receivedBeacon.SourceId,
                            };
                            var site = await _sites.FindAsync(receivedBeacon.SourceId, cancellationToken);
                            if (site != null)
                            {
                                siteInfo.Name = site.Name;
                            }
                            sites.Add(receivedBeacon.SourceId, siteInfo);
                        }
                    }
                }
                item.Site = siteInfo ?? new SiteInfo
                {
                    Id = string.Empty,
                };

                if (!items.ContainsKey(beacon.ProductId))
                    items.Add(beacon.ProductId, new List<DashboardByProductItem> { item });
                else
                {
                    items[beacon.ProductId].Add(item);
                }
            }

            var products = await _products.ListAsync(p => p.ProviderId == providerId, cancellationToken);
            foreach (var product in products)
            {
                var dashboardByProduct = new DashboardByProduct
                {
                    Product = new ProductInfo
                    {
                        Id = product.Id,
                        Name = product.Name,
                    },
                    Beacons = !items.ContainsKey(product.Id)
                        ? new List<DashboardByProductItem>()
                        : items[product.Id]
                };

                result.Add(dashboardByProduct);
            }

            return result;
        }
    }
}
