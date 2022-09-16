using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.Specifications;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services;
using Warehouse.Core.Services.Security;
using Warehouse.Core.UseCases.BeaconTracking.Models;
using Warehouse.Core.UseCases.Management.Models;

namespace Warehouse.Core.UseCases.BeaconTracking.Queries
{
    public class GetDashboardByBeacon : PagingModelBase, ILinqSpecification<BeaconReceivedEntity>, IQuery<IPagedEnumerable<DashboardByBeacon>>
    {
        public string SearchTerm { set; get; }
        public string SiteId { set; get; }
        public string ProductId { set; get; }
        public long ProviderId { set; get; }

        public IQueryable<BeaconReceivedEntity> Apply(IQueryable<BeaconReceivedEntity> query)
        {
            query.Where(b => b.ProviderId == ProviderId)
                .WhereIf(!string.IsNullOrEmpty(SearchTerm),
                    b => b.MacAddress.ToLower().Contains(SearchTerm.ToLower()))
                .WhereIf(!string.IsNullOrEmpty(SiteId), b => b.SourceId == SiteId)
                .WhereIf(!string.IsNullOrEmpty(ProductId), b => true)
                .OrderBy(p => p.MacAddress)
                .Paginate(this);

            return query;
        }
    }

    internal class HandleDashboardByBeacon : IQueryHandler<GetDashboardByBeacon, IPagedEnumerable<DashboardByBeacon>>
    {
        private readonly IReadOnlyRepository<BeaconReceivedEntity> _beaconsReceived;
        private readonly IReadOnlyRepository<WarehouseSiteEntity> _sites;
        private readonly IReadOnlyRepository<BeaconEntity> _beacons;
        private readonly IReadOnlyRepository<ProductEntity> _products;
        private readonly IUserContext _userContext;
        private readonly IMapper _mapper;

        public HandleDashboardByBeacon(
            IReadOnlyRepository<BeaconReceivedEntity> beaconsReceived,
            IReadOnlyRepository<WarehouseSiteEntity> sites,
            IReadOnlyRepository<BeaconEntity> beacons,
            IReadOnlyRepository<ProductEntity> products,
            IMapper mapper, IUserContext userContext)
        {
            _sites = sites;
            _beaconsReceived = beaconsReceived;
            _beacons = beacons;
            _products = products;
            _mapper = mapper;
            _userContext = userContext;
        }

        public async Task<IPagedEnumerable<DashboardByBeacon>> Handle(GetDashboardByBeacon query, CancellationToken cancellationToken)
        {
            query.ProviderId = _userContext.User.Identity.GetProviderId();

            var beacons = await _beaconsReceived.PagedEnumerableAsync(query, cancellationToken);
            
            var data = new List<DashboardByBeacon>();
            foreach (var b in beacons)
            {
                var asset = new DashboardByBeacon
                {
                    MacAddress = b.MacAddress,
                    TimeStamp = b.ReceivedAt,

                    SiteId = b.SourceId
                };

                var site = await _sites.FindAsync(b.SourceId, cancellationToken);
                if (site != null)
                {
                    asset.Site = _mapper.Map<WarehouseSiteDto>(site);
                }

                var productItem = await _beacons.FirstOrDefaultAsync(q => q.Id.Equals(b.MacAddress), cancellationToken);
                if (productItem != null)
                {
                    if (!string.IsNullOrEmpty(productItem.ProductId))
                    {
                        var product = await _products.FirstOrDefaultAsync(p => p.Id == productItem.ProductId, cancellationToken);
                        if (product != null)
                        {
                            asset.Product = _mapper.Map<ProductDto>(product);
                        }
                    }
                }

                data.Add(asset);
            }

            return new PagedEnumerable<DashboardByBeacon>(data, beacons.TotalCount);
        }
    }
}
