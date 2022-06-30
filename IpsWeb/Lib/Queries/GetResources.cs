using System.Reflection;
using IpsWeb.Lib.API.TagHelpers;
using Microsoft.Extensions.Localization;
using Vayosoft.Core.Caching;
using Vayosoft.Core.Helpers;
using Vayosoft.Core.SharedKernel.Queries.Handler;
using Vayosoft.Core.SharedKernel.Queries.Query;

namespace IpsWeb.Lib.Queries
{
    public class GetResources : IQuery<IEnumerable<ResourceGroup>>
    {
        public IEnumerable<string> ResourceNames { get; }

        public GetResources(IEnumerable<string> resourceNames)
        {
            ResourceNames = resourceNames;
        }
        public class ResourcesQueryHandler : IQueryHandler<GetResources, IEnumerable<ResourceGroup>>
        {
            private readonly IStringLocalizerFactory _stringLocalizerFactory;
            private readonly IDistributedMemoryCache _cache;

            public ResourcesQueryHandler(IStringLocalizerFactory stringLocalizerFactory, IDistributedMemoryCache cache)
            {
                _stringLocalizerFactory = stringLocalizerFactory;
                _cache = cache;
            }

            public Task<IEnumerable<ResourceGroup>> Handle(GetResources request, CancellationToken cancellationToken)
            {
                var resourceNames = Guard.NotNull(request.ResourceNames, nameof(request.ResourceNames));
                var groupedResources = resourceNames.Select(x =>
                {
                    return _cache.GetOrCreateExclusive(CacheKey.With<ResourceGroup>(x), options =>
                    {
                        options.SlidingExpiration = TimeSpans.FiveMinutes;

                        IStringLocalizer localizer = _stringLocalizerFactory.Create(x, Assembly.GetEntryAssembly()!.FullName!);
                        return new ResourceGroup { Name = x, Entries = localizer.GetAllStrings(true).ToList() };
                    });
                });

                return Task.FromResult(groupedResources);
            }
        }
    }
}
