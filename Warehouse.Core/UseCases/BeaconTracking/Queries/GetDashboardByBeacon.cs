using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Models;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.BeaconTracking.Models;
using Warehouse.Core.UseCases.Management.Models;

namespace Warehouse.Core.UseCases.BeaconTracking.Queries
{
    public class GetDashboardByBeacon : PagingBase<BeaconReceivedEntity, object>, IQuery<IPagedEnumerable<DashboardByBeacon>>
    {
        public long ProviderId { get; }
        public string FilterString { get; }

        public GetDashboardByBeacon(int page, int take, long providerId, string searchTerm = null)
        {
            Page = page;
            Take = take;

            ProviderId = providerId;
            FilterString = searchTerm;
        }

        public static GetDashboardByBeacon Create(int pageNumber = 1, int pageSize = 20, long providerId = 0, string searchTerm = null)
        {
            return new GetDashboardByBeacon(pageNumber, pageSize, providerId, searchTerm);
        }

        protected override Sorting<BeaconReceivedEntity, object> BuildDefaultSorting() =>
            new(p => p.Id, SortOrder.Desc);

        public void Deconstruct(out int pageNumber, out int pageSize, out long providerId, out string filterString)
        {
            pageNumber = Page;
            pageSize = Take;

            providerId = ProviderId;
            filterString = FilterString;
        }
    }

    internal class HandleDashboardByBeacon : IQueryHandler<GetDashboardByBeacon, IPagedEnumerable<DashboardByBeacon>>
    {
        private readonly IReadOnlyRepository<WarehouseSiteEntity> _siteRepository;
        private readonly IReadOnlyRepository<BeaconReceivedEntity> _beaconReceivedRepository;
        private readonly IReadOnlyRepository<BeaconEntity> _beaconRepository;
        private readonly IReadOnlyRepository<ProductEntity> _productRepository;
        private readonly IMapper _mapper;

        public HandleDashboardByBeacon(
            IReadOnlyRepository<WarehouseSiteEntity> siteRepository,
            IReadOnlyRepository<BeaconReceivedEntity> beaconReceivedRepository,
            IReadOnlyRepository<BeaconEntity> beaconRepository,
            IReadOnlyRepository<ProductEntity> productRepository,
            IMapper mapper)
        {
            _siteRepository = siteRepository;
            _beaconReceivedRepository = beaconReceivedRepository;
            _beaconRepository = beaconRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<IPagedEnumerable<DashboardByBeacon>> Handle(GetDashboardByBeacon query, CancellationToken cancellationToken)
        {
            IPagedEnumerable<BeaconReceivedEntity> result;
            if (!string.IsNullOrEmpty(query.FilterString))
            {
                result = await _beaconReceivedRepository.PagedListAsync(query, e =>
                    //e.ProviderId == query.ProviderId && 
                    e.MacAddress.ToLower().Contains(query.FilterString.ToLower()), cancellationToken);
            }
            else
            {
                result = await _beaconReceivedRepository.PagedListAsync(query,
                    //p => p.ProviderId == query.ProviderId,
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

                var site = await _siteRepository.FindAsync(b.SourceId, cancellationToken);
                if (site != null)
                {
                    asset.Site = _mapper.Map<WarehouseSiteDto>(site);
                }

                var productItem = await _beaconRepository.FirstOrDefaultAsync(q => q.Id.Equals(b.MacAddress), cancellationToken);
                if (productItem != null)
                {
                    if (!string.IsNullOrEmpty(productItem.ProductId))
                    {
                        var product = await _productRepository.FirstOrDefaultAsync(p => p.Id == productItem.ProductId, cancellationToken);
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
