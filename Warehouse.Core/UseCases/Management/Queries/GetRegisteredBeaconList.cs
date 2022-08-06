using Vayosoft.Core.Caching;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Entities.Models;

namespace Warehouse.Core.UseCases.Management.Queries
{
    public class GetRegisteredBeaconList : IQuery<IEnumerable<string>>
    { }

    public class HandleGetRegisteredBeaconList : IQueryHandler<GetRegisteredBeaconList, IEnumerable<string>>
    {
        private readonly IReadOnlyRepository<BeaconRegisteredEntity> _repository;
        private readonly IDistributedMemoryCache _cache;

        public HandleGetRegisteredBeaconList(IReadOnlyRepository<BeaconRegisteredEntity> repository, IDistributedMemoryCache cache, IMapper mapper, IQueryBus queryBus)
        {
            _repository = repository;
            _cache = cache;
        }

        public async Task<IEnumerable<string>> Handle(GetRegisteredBeaconList request, CancellationToken cancellationToken)
        {
            var data = await _cache.GetOrCreateExclusiveAsync(CacheKey.With<BeaconRegisteredEntity>(), async options =>
            {
                options.AbsoluteExpirationRelativeToNow = TimeSpans.FiveMinutes;

                return (await _repository.ListAsync(cancellationToken))
                    .Select(b => b.MacAddress);
            });

            return data;
        }
    }
}
