using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Vayosoft.Core.Utilities;

namespace Warehouse.API.Extensions
{
    public static class DistributedCacheExtension
    {
        public static T Get<T>(this IDistributedCache cache, string key)
        {
            var value = cache.Get(key);
            return value == null ? default : Encoding.UTF8.GetString(value).FromJson<T>();
        }

        public static void Set<T>(this IDistributedCache cache, string key, T value, DateTime expireTime, TimeSpan? sliding = null)
        {
            var json = value.ToJson();
            var options = new DistributedCacheEntryOptions()
            {
                AbsoluteExpiration = expireTime
            };

            if (sliding != null)
            {
                options.SetSlidingExpiration(sliding.Value);
            }

            cache.Set(key, Encoding.UTF8.GetBytes(json), options);
        }
    }
}
