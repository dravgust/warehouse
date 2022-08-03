using Vayosoft.Core.Caching;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Persistence;

namespace Warehouse.Core.UseCases.Management.Queries
{
    public class GetRegisteredBeaconList : IQuery<IEnumerable<string>>
    { }

    public class HandleGetRegisteredBeaconList : IQueryHandler<GetRegisteredBeaconList, IEnumerable<string>>
    {
        private readonly WarehouseStore _store;
        private readonly IDistributedMemoryCache _cache;

        public HandleGetRegisteredBeaconList(WarehouseStore store, IDistributedMemoryCache cache, IMapper mapper, IQueryBus queryBus)
        {
            _store = store;
            _cache = cache;
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
    }
}
