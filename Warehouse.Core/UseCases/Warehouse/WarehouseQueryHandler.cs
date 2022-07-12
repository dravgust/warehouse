using MongoDB.Driver;
using Vayosoft.Core.Caching;
using Vayosoft.Core.Persistence.Queries;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Core.Utilities;
using Vayosoft.Data.MongoDB;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.Products.Models;
using Warehouse.Core.UseCases.Warehouse.Models;
using Warehouse.Core.UseCases.Warehouse.Queries;
using Warehouse.Core.UseCases.Warehouse.Specifications;

namespace Warehouse.Core.UseCases.Warehouse
{
    public class WarehouseQueryHandler : 
        IQueryHandler<GetRegisteredBeaconList, IEnumerable<string>>,
        IQueryHandler<GetProductItems, IPagedEnumerable<ProductItemDto>>
    {
        private readonly IDistributedMemoryCache _cache;
        private readonly IMapper _mapper;
        private readonly IQueryBus _queryBus;
        private readonly IMongoCollection<BeaconRegisteredEntity> _collection;
        private readonly IMongoCollection<ProductEntity> _products;
        private readonly IMongoCollection<BeaconEntity> _productItems;

        public WarehouseQueryHandler(IMongoContext context, IDistributedMemoryCache cache, IMapper mapper, IQueryBus queryBus)
        {
            _collection = context.Database.GetCollection<BeaconRegisteredEntity>(CollectionName.For<BeaconRegisteredEntity>());
            _products = context.Database.GetCollection<ProductEntity>(CollectionName.For<ProductEntity>());
            _cache = cache;
            _mapper = mapper;
            _queryBus = queryBus;
            _productItems = context.Database.GetCollection<BeaconEntity>(CollectionName.For<BeaconEntity>());
        }

        public async Task<IEnumerable<string>> Handle(GetRegisteredBeaconList request, CancellationToken cancellationToken)
        {
            var data = await _cache.GetOrCreateExclusiveAsync(CacheKey.With<BeaconRegisteredEntity>(), async options =>
            {
                options.AbsoluteExpirationRelativeToNow = TimeSpans.FiveMinutes;

                var data = await _collection
                    .FindAsync(Builders<BeaconRegisteredEntity>.Filter.Empty, cancellationToken: cancellationToken);
                return (await data.ToListAsync(cancellationToken: cancellationToken)).Select(b => b.MacAddress);
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

                var productItem =  await _productItems.Find(q => q.Id.Equals(item.MacAddress)).FirstOrDefaultAsync(cancellationToken: cancellationToken);
                if (productItem != null)
                {
                    if (!string.IsNullOrEmpty(productItem.ProductId))
                    {
                        var product = await _products.Find(q => q.Id.Equals(productItem.ProductId))
                            .FirstOrDefaultAsync(cancellationToken: cancellationToken);
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
