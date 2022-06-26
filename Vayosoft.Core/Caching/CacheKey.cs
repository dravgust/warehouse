using System;
using Vayosoft.Core.Extensions;

namespace Vayosoft.Core.Caching
{
    public class CacheKey
    {
        public static string With(params string[] keys) =>
            string.Join("-", keys);

        public static string With<T>(params string[] keys) =>
            With(typeof(T), keys);

        public static string With(Type ownerType, params string[] keys) =>
            With($"{ownerType.GetCacheKey()}:{string.Join("-", keys)}");
    }
}
