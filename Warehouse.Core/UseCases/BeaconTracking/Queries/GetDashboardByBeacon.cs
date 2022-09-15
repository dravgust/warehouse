using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.Specifications;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services;
using Warehouse.Core.Services.Security;
using Warehouse.Core.UseCases.BeaconTracking.Models;
using Warehouse.Core.UseCases.Management.Models;

namespace Warehouse.Core.UseCases.BeaconTracking.Queries
{
    public record GetDashboardByBeacon(int Page, int Size, string SearchTerm, string SiteId, string ProductId)
        : IQuery<IPagedEnumerable<DashboardByBeacon>>;

    internal class HandleDashboardByBeacon : IQueryHandler<GetDashboardByBeacon, IPagedEnumerable<DashboardByBeacon>>
    {
        private readonly IReadOnlyRepositoryBase<BeaconReceivedEntity> _beaconsReceived;
        private readonly IReadOnlyRepositoryBase<WarehouseSiteEntity> _sites;
        private readonly IReadOnlyRepositoryBase<BeaconEntity> _beacons;
        private readonly IReadOnlyRepositoryBase<ProductEntity> _products;
        private readonly IUserContext _userContext;
        private readonly IMapper _mapper;

        public HandleDashboardByBeacon(
            IReadOnlyRepositoryBase<BeaconReceivedEntity> beaconsReceived,
            IReadOnlyRepositoryBase<WarehouseSiteEntity> sites,
            IReadOnlyRepositoryBase<BeaconEntity> beacons,
            IReadOnlyRepositoryBase<ProductEntity> products,
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
            var providerId = _userContext.User.Identity.GetProviderId();

            var spec = SpecificationBuilder<BeaconReceivedEntity>
                .Criteria(b => b.ProviderId == providerId)
                .WhereIf(!string.IsNullOrEmpty(query.SearchTerm),
                    b => b.MacAddress.ToLower().Contains(query.SearchTerm.ToLower()))
                .WhereIf(!string.IsNullOrEmpty(query.SiteId), b => b.SourceId == query.SiteId)
                .WhereIf(!string.IsNullOrEmpty(query.ProductId), b => true)
                .Page(query.Page).PageSize(query.Size)
                .OrderBy(p => p.MacAddress)
                .Build();

            var beacons = await _beaconsReceived.ListAsync(spec, cancellationToken);
            
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
