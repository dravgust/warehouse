using MongoDB.Driver;
using Vayosoft.Core.Caching;
using Vayosoft.Core.Persistence.Queries;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Persistence;
using Warehouse.Core.UseCases.Management.Models;
using Warehouse.Core.UseCases.Management.Specifications;

namespace Warehouse.Core.UseCases.Management.Queries
{
    public class WarehouseQueryHandler :
        IQueryHandler<GetRegisteredBeaconList, IEnumerable<string>>,
        IQueryHandler<GetProductItems, IPagedEnumerable<ProductItemDto>>
    {
        private readonly WarehouseStore _store;
        private readonly IDistributedMemoryCache _cache;
        private readonly IMapper _mapper;
        private readonly IQueryBus _queryBus;

        public WarehouseQueryHandler(WarehouseStore store, IDistributedMemoryCache cache, IMapper mapper, IQueryBus queryBus)
        {
            _store = store;
            _cache = cache;
            _mapper = mapper;
            _queryBus = queryBus;
        }

        public async Task<IEnumerable<string>> Handle(GetRegisteredBeaconList request, CancellationToken cancellationToken)
        {
            var data = await _cache.GetOrCreateExclusiveAsync(CacheKey.With<BeaconRegisteredEntity>(), async options =>
            {
                options.AbsoluteExpirationRelativeToNow = TimeSpans.FiveMinutes;

                return (await _store.ListAsync<BeaconRegisteredEntity>(cancellationToken))
                    .Select(b => b.MacAddress);
            });

            return data;
        }

        public async Task<IPagedEnumerable<ProductItemDto>> Handle(GetProductItems request, CancellationToken cancellationToken)
        {
            var spec = new WarehouseProductSpec(request.Page, request.Size, request.SearchTerm);
            var query = new SpecificationQuery<WarehouseProductSpec, IPagedEnumerable<BeaconRegisteredEntity>>(spec);

            var result = await _queryBus.Send(query, cancellationToken);
            var data = new List<ProductItemDto>();
            foreach (var item in result)
            {
                var dto = new ProductItemDto
                {
                    MacAddress = item.MacAddress,
                };

                var productItem = await _store.FirstOrDefaultAsync<BeaconEntity>(q => q.Id.Equals(item.MacAddress), cancellationToken);
                if (productItem != null)
                {
                    if (!string.IsNullOrEmpty(productItem.ProductId))
                    {
                        var product = await _store.FindAsync<ProductEntity>(productItem.ProductId, cancellationToken);
                        if (product != null)
                            dto.Product = _mapper.Map<ProductDto>(product);
                    }

                    dto.Name = productItem.Name;
                    dto.Metadata = productItem.Metadata;
                }

                data.Add(dto);
            }

            return new PagedEnumerable<ProductItemDto>(data, result.TotalCount);
        }
    }
}
