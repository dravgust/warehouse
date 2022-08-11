using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services;
using Warehouse.Core.UseCases.BeaconTracking.Models;
using Warehouse.Core.UseCases.Management.Models;
using Warehouse.Core.Utilities;

namespace Warehouse.Core.UseCases.BeaconTracking.Queries
{
    public class GetDashboardByBeacon : PagingBase<BeaconReceivedEntity, object>, IQuery<IPagedEnumerable<DashboardByBeacon>>
    {
        public string FilterString { get; }

        public GetDashboardByBeacon(int page, int size, string searchTerm = null)
        {
            Page = page;
            Size = size;

            FilterString = searchTerm;
        }

        public static GetDashboardByBeacon Create(int pageNumber = 1, int pageSize = 20, string searchTerm = null)
        {
            return new GetDashboardByBeacon(pageNumber, pageSize, searchTerm);
        }

        protected override Sorting<BeaconReceivedEntity, object> BuildDefaultSorting() =>
            new(p => p.Id, SortOrder.Desc);

        public void Deconstruct(out int pageNumber, out int pageSize, out string filterString)
        {
            pageNumber = Page;
            pageSize = Size;

            filterString = FilterString;
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
            var providerId = _userContext.User.Identity.GetProviderId();
            IPagedEnumerable<BeaconReceivedEntity> result;
            if (!string.IsNullOrEmpty(query.FilterString))
            {
                result = await _beaconsReceived.PagedListAsync(query, e =>
                    e.ProviderId == providerId && 
                    e.MacAddress.ToLower().Contains(query.FilterString.ToLower()), cancellationToken);
            }
            else
            {
                result = await _beaconsReceived.PagedListAsync(query, p => p.ProviderId == providerId,
                    cancellationToken);
            }

            var data = new List<DashboardByBeacon>();
            foreach (var b in result)
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

            return new PagedEnumerable<DashboardByBeacon>(data, result.TotalCount);
        }
    }
}
