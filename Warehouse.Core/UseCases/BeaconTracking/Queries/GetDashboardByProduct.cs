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
        private readonly IReadOnlyRepository<BeaconReceivedEntity> _beaconReceivedRepository;
        private readonly IReadOnlyRepository<WarehouseSiteEntity> _siteRepository;
        private readonly IReadOnlyRepository<BeaconEntity> _beaconRepository;
        private readonly IReadOnlyRepository<ProductEntity> _productRepository;
        private readonly IUserContext _userContext;

        public HandleGetDashboardByProduct(
            IReadOnlyRepository<BeaconReceivedEntity> beaconReceivedRepository,
            IReadOnlyRepository<WarehouseSiteEntity> siteRepository,
            IReadOnlyRepository<BeaconEntity> beaconRepository,
            IReadOnlyRepository<ProductEntity> productRepository,
            IUserContext userContext)
        {
            _beaconReceivedRepository = beaconReceivedRepository;
            _siteRepository = siteRepository;
            _beaconRepository = beaconRepository;
            _productRepository = productRepository;
            _userContext = userContext;
        }

        public async Task<IEnumerable<DashboardByProduct>> Handle(GetDashboardByProduct request, CancellationToken cancellationToken)
        {
            var result = await _beaconReceivedRepository.ListAsync(cancellationToken);
            var providerId = _userContext.User.Identity.GetProviderId();
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

                var productItem = await _beaconRepository.FirstOrDefaultAsync(q => q.Id.Equals(b.MacAddress), cancellationToken);
                if (productItem != null)
                {
                    if (!string.IsNullOrEmpty(productItem.ProductId))
                    {
                        var product = await _productRepository.FirstOrDefaultAsync(p => p.Id == productItem.ProductId, cancellationToken);
                        if (product != null)
                        {
                            productInfo.Id = product.Id;
                            productInfo.Name = product.Name;
                        }
                    }
                }

                if (!store.ContainsKey((productInfo.Id, siteInfo.Id)))
                {
                    var site = await _siteRepository.FindAsync(b.SourceId, cancellationToken);
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
