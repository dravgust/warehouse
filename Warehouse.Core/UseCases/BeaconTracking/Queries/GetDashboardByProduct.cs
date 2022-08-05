using System.Collections.ObjectModel;
using Vayosoft.Core.Queries;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Persistence;
using Warehouse.Core.UseCases.BeaconTracking.Models;

namespace Warehouse.Core.UseCases.BeaconTracking.Queries
{
    public class GetDashboardByProduct : IQuery<IEnumerable<DashboardByProduct>>
    { }

    internal class HandleGetDashboardByProduct : IQueryHandler<GetDashboardByProduct, IEnumerable<DashboardByProduct>>
    {
        private readonly WarehouseDataStore _store;

        public HandleGetDashboardByProduct(WarehouseDataStore store)
        {
            _store = store;
        }

        public async Task<IEnumerable<DashboardByProduct>> Handle(GetDashboardByProduct request, CancellationToken cancellationToken)
        {
            var result = await _store.ListAsync<BeaconReceivedEntity>(cancellationToken);

            var store = new SortedDictionary<(string, string), DashboardByProduct>(Comparer<(string, string)>.Create((x, y) => y.CompareTo(x)));

            foreach (var b in result)
            {
                var productInfo = new ProductInfo
                {
                    Id = string.Empty
                };
                var siteInfo = new SiteInfo
                {
                    Id = b.SourceId
                };

                var productItem = await _store.FirstOrDefaultAsync<BeaconEntity>(q => q.Id.Equals(b.MacAddress), cancellationToken);
                if (productItem != null)
                {
                    if (!string.IsNullOrEmpty(productItem.ProductId))
                    {
                        var product = await _store.FirstOrDefaultAsync<ProductEntity>(p => p.Id == productItem.ProductId, cancellationToken);
                        if (product != null)
                        {
                            productInfo.Id = product.Id;
                            productInfo.Name = product.Name;
                        }
                    }
                }

                if (!store.ContainsKey((productInfo.Id, siteInfo.Id)))
                {
                    var site = await _store.FindAsync<WarehouseSiteEntity>(b.SourceId, cancellationToken);
                    siteInfo.Name = site?.Name;

                    var asset = new DashboardByProduct
                    {
                        Product = productInfo,
                        Site = siteInfo,
                        Beacons = new Collection<BeaconInfo>
                       {
                           new()
                           {
                               MacAddress = b.MacAddress,
                               Name = productItem?.Name
                           }
                       },
                    };
                    store[(productInfo.Id, siteInfo.Id)] = asset;
                }
                else
                {
                    store[(productInfo.Id, siteInfo.Id)].Beacons.Add(new BeaconInfo
                    {
                        MacAddress = b.MacAddress,
                        Name = productItem?.Name
                    });
                }

            }


            return store.Values;
        }
    }
}
