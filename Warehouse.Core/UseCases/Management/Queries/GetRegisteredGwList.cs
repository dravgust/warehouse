using Microsoft.EntityFrameworkCore;
using Vayosoft.Core.Caching;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services;

namespace Warehouse.Core.UseCases.Management.Queries
{
    public record GetRegisteredGwList : IQuery<IEnumerable<string>>;

    public sealed class RegisteredGwQueryHandler : IQueryHandler<GetRegisteredGwList, IEnumerable<string>>
    {
        private readonly IDistributedMemoryCache _cache;
        private readonly IUserContext _userContext;
        private readonly ILinqProvider _linqProvider;

        public RegisteredGwQueryHandler(ILinqProvider linqProvider, IDistributedMemoryCache cache,
            IUserContext userContext)
        {
            _linqProvider = linqProvider;
            _cache = cache;
            _userContext = userContext;
        }

        public async Task<IEnumerable<string>> Handle(GetRegisteredGwList request, CancellationToken cancellationToken)
        {
            var data = await _cache.GetOrCreateExclusiveAsync(CacheKey.With<DeviceEntity>(), async options =>
            {
                options.AbsoluteExpirationRelativeToNow = TimeSpans.FiveMinutes;

                var data = await _linqProvider
                    .Where<DeviceEntity>(d => d.ProviderId == 2)
                    .ToListAsync(cancellationToken: cancellationToken);

                return data.Select(d => d.MacAddress).OrderBy(macAddress => macAddress);
            });

            return data;
        }
    }
}