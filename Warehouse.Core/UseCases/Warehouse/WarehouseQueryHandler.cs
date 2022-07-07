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
    public class WarehouseQueryHandler : IQueryHandler<GetRegisteredBeaconList, IEnumerable<string>>, IQueryHandler<GetProductItems, IPagedEnumerable<ProductItem>>
    {
        private readonly IDistributedMemoryCache _cache;
        private readonly IMapper _mapper;
        private readonly IQueryBus _queryBus;
        private readonly IMongoCollection<BeaconRegisteredEntity> _collection;
        private readonly IMongoCollection<ProductEntity> _products;

        public WarehouseQueryHandler(IMongoContext context, IDistributedMemoryCache cache, IMapper mapper, IQueryBus queryBus)
        {
            _collection = context.Database.GetCollection<BeaconRegisteredEntity>(CollectionName.For<BeaconRegisteredEntity>());
            _products = context.Database.GetCollection<ProductEntity>(CollectionName.For<ProductEntity>());
            _cache = cache;
            _mapper = mapper;
            _queryBus = queryBus;
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

        public async Task<IPagedEnumerable<ProductItem>> Handle(GetProductItems request, CancellationToken cancellationToken)
        {
            var spec = new WarehouseProductSpec(request.Page, request.Size, request.SearchTerm);
            var query = new SpecificationQuery<WarehouseProductSpec, IPagedEnumerable<BeaconRegisteredEntity>>(spec);

            var result = await _queryBus.Send(query, cancellationToken);
            var data = new List<ProductItem>();
            foreach (var item in result)
            {
                var productItem = new ProductItem
                {
                    MacAddress = item.MacAddress,

                };

                if (!string.IsNullOrEmpty(item.ProductId))
                {
                    var product = await _products.Find(q => q.Id.Equals(item.ProductId)).FirstOrDefaultAsync(cancellationToken: cancellationToken);
                    productItem.Product = _mapper.Map<ProductDto>(product);
                }

                data.Add(productItem);
            }

            return new PagedEnumerable<ProductItem>(data, result.TotalCount);
        }
    }
}
