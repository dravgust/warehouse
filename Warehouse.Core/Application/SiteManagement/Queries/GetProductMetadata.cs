using Vayosoft.Core.Caching;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.Application.SiteManagement.Queries
{
    public sealed record GetProductMetadata : IQuery<Metadata>
    { }

    internal sealed class HandleGetProductMetadata : IQueryHandler<GetProductMetadata, Metadata>
    {
        private readonly IDistributedMemoryCache _cache;
        private readonly IRepositoryBase<FileEntity> _fileRepository;

        public HandleGetProductMetadata(IRepositoryBase<FileEntity> fileRepository, IDistributedMemoryCache cache)
        {
            _fileRepository = fileRepository;
            _cache = cache;
        }

        public async Task<Metadata> Handle(GetProductMetadata request, CancellationToken cancellationToken)
        {
            var data = await _cache.GetOrCreateExclusiveAsync(CacheKey.With<Metadata>(), async options =>
            {
                options.SlidingExpiration = TimeSpans.FiveMinutes;
                var entity = await _fileRepository.FindAsync("product_metadata", cancellationToken);
                Metadata data = null;
                if (!string.IsNullOrEmpty(entity?.Content))
                    data = entity.Content.FromJson<Metadata>();

                return data;
            });

            return data;
        }
    }
}
