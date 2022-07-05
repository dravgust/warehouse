using Vayosoft.Core.Persistence;
using Vayosoft.Core.Persistence.Queries;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.IPS.Models;
using Warehouse.Core.UseCases.IPS.Queries;
using Warehouse.Core.UseCases.Products.Models;
using Warehouse.Core.UseCases.Warehouse.Models;
using Warehouse.Core.UseCases.Warehouse.Specifications;

namespace Warehouse.Core.UseCases.IPS
{
    public class AssetsQueryHandler : IQueryHandler<GetAssets, IPagedEnumerable<AssetDto>>
    {
        private readonly IQueryBus _queryBus;
        private readonly IRepository<WarehouseSiteEntity, string> _siteRepository;
        private readonly IReadOnlyRepository<ProductEntity> _productRepository;
        private readonly IMapper _mapper;

        public AssetsQueryHandler(IQueryBus queryBus,
            IRepository<WarehouseSiteEntity, string> siteRepository,
            IReadOnlyRepository<ProductEntity> productRepository,
            IMapper mapper)
        {
            _queryBus = queryBus;
            _siteRepository = siteRepository;
            _productRepository = productRepository;
            _mapper = mapper;
        }

        public async Task<IPagedEnumerable<AssetDto>> Handle(GetAssets request, CancellationToken cancellationToken)
        {
            var spec = new BeaconPositionSpec(request.Page, request.Size, request.SearchTerm);
            var query = new SpecificationQuery<BeaconPositionSpec, IPagedEnumerable<BeaconIndoorPositionEntity>>(spec);
            var result = await _queryBus.Send(query, cancellationToken);

            var data = new List<AssetDto>();
            foreach (var b in result)
            {
                var asset = new AssetDto
                {
                    MacAddress = b.MacAddress,
                    TimeStamp = b.TimeStamp,

                    SiteId = b.SiteId
                };

                var site = await _siteRepository.FindAsync(b.SiteId, cancellationToken);
                if (site != null)
                {
                    asset.Site = _mapper.Map<WarehouseSiteDto>(site);
                }

                var product = (await _productRepository.FindAllAsync(p => p.MacAddress == b.MacAddress, cancellationToken))
                    .FirstOrDefault();
                if (product != null)
                {
                    asset.Product = _mapper.Map<ProductDto>(product);
                }

                data.Add(asset);
            }

            return new PagedEnumerable<AssetDto>(data, result.TotalCount);
        }
    }
}
