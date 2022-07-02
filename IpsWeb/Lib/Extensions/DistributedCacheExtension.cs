using System.Text;
using Microsoft.Extensions.Caching.Distributed;
using Newtonsoft.Json;

namespace IpsWeb.Lib.Extensions
{
    public static class DistributedCacheExtension
    {
        public static T? Get<T>(this IDistributedCache cache, string key)
        {
            var value = cache.Get(key);
            return value == null ? default : JsonConvert.DeserializeObject<T>(Encoding.UTF8.GetString(value));
        }

        public static void Set<T>(this IDistributedCache cache, string key, T value, DateTime expireTime, TimeSpan? sliding = null)
        {
            var json = JsonConvert.SerializeObject(value);
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
