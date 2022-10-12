using Microsoft.Extensions.Caching.Memory;

namespace Vayosoft.Core.Caching
{
    public interface IDistributedMemoryCache : IMemoryCache
    {
        MemoryCacheEntryOptions GetDefaultCacheEntryOptions();
    }
}
