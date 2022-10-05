using Vayosoft.Core.Caching;
using Vayosoft.Core.Queries;
using Vayosoft.Core.Specifications;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Domain.Entities;
using Warehouse.Core.Persistence;
using Warehouse.Core.Services;
using Warehouse.Core.Services.Security;

namespace Warehouse.Core.UseCases.Management.Queries
{
    public class GetRegisteredBeaconList : IQuery<IEnumerable<string>>
    { }

    public class HandleGetRegisteredBeaconList : IQueryHandler<GetRegisteredBeaconList, IEnumerable<string>>
    {
        private readonly WarehouseStore _repository;
        private readonly IDistributedMemoryCache _cache;
        private readonly IUserContext _userContext;

        public HandleGetRegisteredBeaconList(WarehouseStore repository, IDistributedMemoryCache cache, IUserContext userContext)
        {
            _repository = repository;
            _cache = cache;
            _userContext = userContext;
        }

        public async Task<IEnumerable<string>> Handle(GetRegisteredBeaconList request, CancellationToken cancellationToken)
        {
            var providerId = _userContext.User.Identity.GetProviderId();
            var data = await _cache.GetOrCreateExclusiveAsync(CacheKey.With<TrackedItem>(providerId.ToString()), async options =>
            {
                options.AbsoluteExpirationRelativeToNow = TimeSpans.FiveMinutes;
                var spec = new Specification<TrackedItem>(s => s.ProviderId == providerId);
                return (await _repository.TrackedItems.ListAsync(spec, cancellationToken))
                    .Select(b => b.Id);
            });

            return data;
        }
    }
}
