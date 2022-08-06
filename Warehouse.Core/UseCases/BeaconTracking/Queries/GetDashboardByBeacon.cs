using Vayosoft.Core.Persistence;
using Vayosoft.Core.Persistence.Queries;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Persistence;
using Warehouse.Core.UseCases.BeaconTracking.Models;
using Warehouse.Core.UseCases.BeaconTracking.Specifications;
using Warehouse.Core.UseCases.Management.Models;

namespace Warehouse.Core.UseCases.BeaconTracking.Queries
{
    public class GetDashboardByBeacon : IQuery<IPagedEnumerable<DashboardByBeacon>>
    {
        public int Page { set; get; }
        public int Size { set; get; }
        public string SearchTerm { set; get; }
    }

    internal class HandleDashboardByBeacon : IQueryHandler<GetDashboardByBeacon, IPagedEnumerable<DashboardByBeacon>>
    {
        private readonly IReadOnlyRepository<WarehouseSiteEntity> _siteRepository;
        private readonly IReadOnlyRepository<BeaconEntity> _beaconRepository;
        private readonly IReadOnlyRepository<ProductEntity> _productRepository;
        private readonly IQueryBus _queryBus;
        private readonly IMapper _mapper;

        public HandleDashboardByBeacon(
            IReadOnlyRepository<WarehouseSiteEntity> siteRepository,
            IReadOnlyRepository<BeaconEntity> beaconRepository,
            IReadOnlyRepository<ProductEntity> productRepository,
            IQueryBus queryBus, IMapper mapper)
        {
            _siteRepository = siteRepository;
            _beaconRepository = beaconRepository;
            _productRepository = productRepository;
            _queryBus = queryBus;
            _mapper = mapper;
        }

        public async Task<IPagedEnumerable<DashboardByBeacon>> Handle(GetDashboardByBeacon request, CancellationToken cancellationToken)
        {
            var spec = new BeaconPositionSpec(request.Page, request.Size, request.SearchTerm);
            var query = new SpecificationQuery<BeaconPositionSpec, IPagedEnumerable<BeaconReceivedEntity>>(spec);
            var result = await _queryBus.Send(query, cancellationToken);

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
