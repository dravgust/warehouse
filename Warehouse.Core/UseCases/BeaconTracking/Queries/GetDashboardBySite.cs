using Vayosoft.Core.Queries;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Persistence;
using Warehouse.Core.UseCases.BeaconTracking.Models;

namespace Warehouse.Core.UseCases.BeaconTracking.Queries;

public class GetDashboardBySite : IQuery<IEnumerable<DashboardBySite>>
{ }

public class HandleGetDashboardBySite : IQueryHandler<GetDashboardBySite, IEnumerable<DashboardBySite>>
{
    private readonly WarehouseStore _store;

    public HandleGetDashboardBySite(WarehouseStore store)
    {
        _store = store;
    }

    public async Task<IEnumerable<DashboardBySite>> Handle(GetDashboardBySite request, CancellationToken cancellationToken)
    {
        var statusEntities = await _store.ListAsync<IndoorPositionStatusEntity>(cancellationToken);

        var result = new List<DashboardBySite>();
        foreach (var s in statusEntities)
        {
            var site = await _store.FindAsync<WarehouseSiteEntity>(s.Id, cancellationToken);
            if (site != null)
            {
                var info = new DashboardBySite
                {
                    Site = new SiteInfo
                    {
                        Id = site.Id,
                        Name = site.Name
                    },
                    In = new List<DashboardBySiteItem>(),
                    Out = new List<DashboardBySiteItem>()
                };

                foreach (var macAddress in s.In)
                {
                    var beaconPositionInfo = new DashboardBySiteItem
                    {
                        Product = new ProductInfo
                        {
                            Id = string.Empty
                        },
                        Beacon = new BeaconInfo
                        {
                            MacAddress = macAddress
                        }
                    };

                    var productItem = await _store.FirstOrDefaultAsync<BeaconEntity>(q => q.Id.Equals(macAddress), cancellationToken);
                    if (productItem != null)
                    {
                        beaconPositionInfo.Beacon.Name = productItem.Name;

                        if (!string.IsNullOrEmpty(productItem.ProductId))
                        {
                            var product = await _store.FirstOrDefaultAsync<ProductEntity>(p => p.Id == productItem.ProductId, cancellationToken);
                            if (product != null)
                            {
                                beaconPositionInfo.Product.Id = product.Id;
                                beaconPositionInfo.Product.Name = product.Name;
                            }
                        }
                    }

                    info.In.Add(beaconPositionInfo);
                }

                foreach (var macAddress in s.Out)
                {
                    var beaconPositionInfo = new DashboardBySiteItem
                    {
                        Product = new ProductInfo
                        {
                            Id = string.Empty
                        },
                        Beacon = new BeaconInfo
                        {
                            MacAddress = macAddress
                        }
                    };

                    var productItem = await _store.FirstOrDefaultAsync<BeaconEntity>(q => q.Id.Equals(macAddress), cancellationToken);
                    if (productItem != null)
                    {
                        beaconPositionInfo.Beacon.Name = productItem.Name;

                        if (!string.IsNullOrEmpty(productItem.ProductId))
                        {
                            var product = await _store.FirstOrDefaultAsync<ProductEntity>(p => p.Id == productItem.ProductId, cancellationToken);
                            if (product != null)
                            {
                                beaconPositionInfo.Product.Id = product.Id;
                                beaconPositionInfo.Product.Name = product.Name;
                            }
                        }
                    }

                    info.Out.Add(beaconPositionInfo);
                }

                result.Add(info);
            }
        }

        return result;
    }
}