using Vayosoft.Core.Caching;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Queries.Handler;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.Persistence;
using Warehouse.Core.UseCases.Products.Queries;

namespace Warehouse.Core.UseCases.Products
{
    public class ProductQueryHandler : IQueryHandler<GetProductMetadata, ProductMetadata>
    {
        private readonly IDistributedMemoryCache _cache;
        private readonly ICriteriaRepository<FileEntity, string> _fileRepository;

        public ProductQueryHandler(ICriteriaRepository<FileEntity, string> fileRepository, IDistributedMemoryCache cache)
        {
            _fileRepository = fileRepository;
            _cache = cache;
        }

        public async Task<ProductMetadata> Handle(GetProductMetadata request, CancellationToken cancellationToken)
        {
            var data = await _cache.GetOrCreateExclusiveAsync(CacheKey.With<ProductMetadata>(), async options =>
            {
                options.SlidingExpiration = TimeSpans.FiveMinutes;
                var entity = await _fileRepository.GetAsync(nameof(ProductMetadata), cancellationToken);
                ProductMetadata? data = null;
                if (!string.IsNullOrEmpty(entity?.Content))
                    data = entity.Content.FromJson<ProductMetadata>();

                return data;
            });

            return data;
        }
    }
}
