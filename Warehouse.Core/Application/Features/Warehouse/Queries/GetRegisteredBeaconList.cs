using MongoDB.Driver;
using Vayosoft.Core.Caching;
using Vayosoft.Core.Helpers;
using Vayosoft.Core.SharedKernel.Queries.Handler;
using Vayosoft.Core.SharedKernel.Queries.Query;
using Vayosoft.Data.MongoDB;
using Warehouse.Core.Domain.Entities;
using Warehouse.Core.Domain.ValueObjects;

namespace Warehouse.Core.Application.Features.Warehouse.Queries
{
    public class GetRegisteredBeaconList : IQuery<IEnumerable<string>>
    {
        public class RegisteredBeaconQueryHandler : IQueryHandler<GetRegisteredBeaconList, IEnumerable<string>>
        {
            private readonly IDistributedMemoryCache _cache;
            private readonly IMongoCollection<BeaconRegisteredEntity> _collection;

            public RegisteredBeaconQueryHandler(IMongoContext context, IDistributedMemoryCache cache)
            {
                _collection = context.Database.GetCollection<BeaconRegisteredEntity>(CollectionName.For<BeaconRegisteredEntity>());
                _cache = cache;
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
        }
    }
}
