using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services.Session;
using Warehouse.Core.UseCases.BeaconTracking.Models;

namespace Warehouse.Core.UseCases.BeaconTracking.Queries;

public class GetDashboardBySite : IQuery<IEnumerable<DashboardBySite>>
{ }

public class HandleGetDashboardBySite : IQueryHandler<GetDashboardBySite, IEnumerable<DashboardBySite>>
{
    private readonly IReadOnlyRepository<IndoorPositionStatusEntity> _statusRepository;
    private readonly IReadOnlyRepository<WarehouseSiteEntity> _siteRepository;
    private readonly IReadOnlyRepository<BeaconEntity> _beaconRepository;
    private readonly IReadOnlyRepository<ProductEntity> _productRepository;
    private readonly ISessionProvider _session;

    public HandleGetDashboardBySite(
        IReadOnlyRepository<IndoorPositionStatusEntity> statusRepository,
        IReadOnlyRepository<WarehouseSiteEntity> siteRepository,
        IReadOnlyRepository<BeaconEntity> beaconRepository,
        IReadOnlyRepository<ProductEntity> productRepository,
        ISessionProvider session)
    {
        _statusRepository = statusRepository;
        _siteRepository = siteRepository;
        _beaconRepository = beaconRepository;
        _productRepository = productRepository;
        _session = session;
    }

    public async Task<IEnumerable<DashboardBySite>> Handle(GetDashboardBySite request, CancellationToken cancellationToken)
    {
        var statusEntities = await _statusRepository.ListAsync(cancellationToken);
        var providerId = _session.GetInt64(nameof(IProvider.ProviderId));
        var result = new List<DashboardBySite>();
        foreach (var s in statusEntities)
        {
            var site = await _siteRepository.FindAsync(s.Id, cancellationToken);
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

                    var productItem = await _beaconRepository.FirstOrDefaultAsync(q => q.Id.Equals(macAddress), cancellationToken);
                    if (productItem != null)
                    {
                        beaconPositionInfo.Beacon.Name = productItem.Name;

                        if (!string.IsNullOrEmpty(productItem.ProductId))
                        {
                            var product = await _productRepository.FirstOrDefaultAsync(p => p.Id == productItem.ProductId, cancellationToken);
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

                    var productItem = await _beaconRepository.FirstOrDefaultAsync(q => q.Id.Equals(macAddress), cancellationToken);
                    if (productItem != null)
                    {
                        beaconPositionInfo.Beacon.Name = productItem.Name;

                        if (!string.IsNullOrEmpty(productItem.ProductId))
                        {
                            var product = await _productRepository.FirstOrDefaultAsync(p => p.Id == productItem.ProductId, cancellationToken);
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