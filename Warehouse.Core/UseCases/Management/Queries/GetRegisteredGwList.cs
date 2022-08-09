using Microsoft.EntityFrameworkCore;
using Vayosoft.Core.Caching;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.Queries;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services.Session;

namespace Warehouse.Core.UseCases.Management.Queries
{
    public class GetRegisteredGwList : IQuery<IEnumerable<string>>
    {
        public class RegisteredGwQueryHandler : IQueryHandler<GetRegisteredGwList, IEnumerable<string>>
        {
            private readonly IDistributedMemoryCache _cache;
            private readonly ISessionProvider _session;
            private readonly ILinqProvider _linqProvider;

            public RegisteredGwQueryHandler(ILinqProvider linqProvider, IDistributedMemoryCache cache, ISessionProvider session)
            {
                _linqProvider = linqProvider;
                _cache = cache;
                _session = session;
            }

            public async Task<IEnumerable<string>> Handle(GetRegisteredGwList request, CancellationToken cancellationToken)
            {
                var data = await _cache.GetOrCreateExclusiveAsync(CacheKey.With<DeviceEntity>(), async options =>
                {
                    options.AbsoluteExpirationRelativeToNow = TimeSpans.FiveMinutes;

                    var data = await _linqProvider
                        .AsQueryable<DeviceEntity>()
                        .Where(d => d.ProviderId == 2)
                        .ToListAsync(cancellationToken: cancellationToken);

                    return data.Select(d => d.MacAddress).OrderBy(macAddress => macAddress);
                });

                return data;
            }
        }
    }
}
