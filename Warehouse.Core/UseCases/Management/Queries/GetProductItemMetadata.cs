using Vayosoft.Core.Caching;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Entities.Models;

namespace Warehouse.Core.UseCases.Management.Queries
{
    public sealed record GetProductItemMetadata : IQuery<Metadata>
    { }

    internal sealed class HandlerGetProductItemMetadata : IQueryHandler<GetProductItemMetadata, Metadata>
    {
        private readonly IDistributedMemoryCache _cache;
        private readonly IRepositoryBase<FileEntity> _fileRepository;

        public HandlerGetProductItemMetadata(IRepositoryBase<FileEntity> fileRepository, IDistributedMemoryCache cache)
        {
            _fileRepository = fileRepository;
            _cache = cache;
        }

        public async Task<Metadata> Handle(GetProductItemMetadata request, CancellationToken cancellationToken)
        {
            var data = await _cache.GetOrCreateExclusiveAsync(CacheKey.With<Metadata>("beacon"), async options =>
            {
                options.SlidingExpiration = TimeSpans.FiveMinutes;
                var entity = await _fileRepository.FindAsync("beacon_metadata", cancellationToken);
                Metadata data = null;
                if (!string.IsNullOrEmpty(entity?.Content))
                    data = entity.Content.FromJson<Metadata>();

                return data;
            });

            return data;
        }
    }
}
