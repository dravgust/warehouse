﻿using Vayosoft.Caching;
using Vayosoft.Persistence;
using Vayosoft.Queries;
using Vayosoft.Utilities;
using Warehouse.Core.Domain.Entities;
using Warehouse.Core.Domain.ValueObjects;

namespace Warehouse.Core.Application.SiteManagement.Queries
{
    public sealed record GetProductMetadata : IQuery<Metadata>
    { }

    internal sealed class HandleGetProductMetadata : IQueryHandler<GetProductMetadata, Metadata>
    {
        private readonly IDistributedMemoryCache _cache;
        private readonly IRepository<FileEntity> _fileRepository;

        public HandleGetProductMetadata(IRepository<FileEntity> fileRepository, IDistributedMemoryCache cache)
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
                {
                    data = entity.Content.FromJson<Metadata>();
                }

                return data;
            });

            return data;
        }
    }
}
