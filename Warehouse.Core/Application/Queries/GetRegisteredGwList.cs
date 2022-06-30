using Microsoft.EntityFrameworkCore;
using Vayosoft.Core.Caching;
using Vayosoft.Core.Helpers;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Queries.Handler;
using Vayosoft.Core.SharedKernel.Queries.Query;
using Warehouse.Core.Domain.Entities;
using Warehouse.Core.Domain.ValueObjects;

namespace Warehouse.Core.Application.Queries
{
    public class GetRegisteredGwList : IQuery<IEnumerable<string>>
    {
        public class RegisteredGwQueryHandler : IQueryHandler<GetRegisteredGwList, IEnumerable<string>>
        {
            private readonly IDistributedMemoryCache _cache;
            private readonly ILinqProvider _linqProvider;

            public RegisteredGwQueryHandler(ILinqProvider linqProvider, IDistributedMemoryCache cache)
            {
                _linqProvider = linqProvider;
                _cache = cache;
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
