using Vayosoft.Core.Caching;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.Specifications;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services;
using Warehouse.Core.Services.Security;

namespace Warehouse.Core.UseCases.Management.Queries
{
    public class GetRegisteredBeaconList : IQuery<IEnumerable<string>>
    { }

    public class HandleGetRegisteredBeaconList : IQueryHandler<GetRegisteredBeaconList, IEnumerable<string>>
    {
        private readonly IReadOnlyRepository<BeaconRegisteredEntity> _repository;
        private readonly IDistributedMemoryCache _cache;
        private readonly IUserContext _userContext;

        public HandleGetRegisteredBeaconList(IReadOnlyRepository<BeaconRegisteredEntity> repository, IDistributedMemoryCache cache, IUserContext userContext)
        {
            _repository = repository;
            _cache = cache;
            _userContext = userContext;
        }

        public async Task<IEnumerable<string>> Handle(GetRegisteredBeaconList request, CancellationToken cancellationToken)
        {
            var providerId = _userContext.User.Identity.GetProviderId();
            var data = await _cache.GetOrCreateExclusiveAsync(CacheKey.With<BeaconRegisteredEntity>(providerId.ToString()), async options =>
            {
                options.AbsoluteExpirationRelativeToNow = TimeSpans.FiveMinutes;
                var spec = SpecificationBuilder<BeaconRegisteredEntity>.Query(s => s.ProviderId == providerId);
                return (await _repository.ListAsync(spec, cancellationToken))
                    .Select(b => b.MacAddress);
            });

            return data;
        }
    }
}
