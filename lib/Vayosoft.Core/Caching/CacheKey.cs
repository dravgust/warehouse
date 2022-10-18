using Vayosoft.Core.Utilities;

namespace Vayosoft.Core.Caching
{
    public class CacheKey
    {
        public static string With(params string[] keys) =>
            string.Join("-", keys);

        public static string With(string key) => key;

        public static string With(string key1, string key2) =>
            $"{key1}-{key2}";

        public static string With<T>(params string[] keys) =>
            With(typeof(T), keys);

        public static string With<T>() =>
            With(typeof(T), string.Empty);

        public static string With<T>(string key) =>
            With(typeof(T), key);

        public static string With<T>(string key1, string key2) =>
            With(typeof(T), key1, key2);

        public static string With(Type ownerType, string key) =>
            With($"{ownerType.GetCacheKey()}:{key}");

        public static string With(Type ownerType, string key1, string key2) =>
            With($"{ownerType.GetCacheKey()}:{key1}-{key2}");

        public static string With(Type ownerType, params string[] keys) =>
            With($"{ownerType.GetCacheKey()}:{string.Join("-", keys)}");
    }
}
