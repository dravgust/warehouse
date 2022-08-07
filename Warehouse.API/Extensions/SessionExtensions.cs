namespace Warehouse.API.Extensions
{
    using System.Text.Json;

    public static class SessionExtensions
    {
        public static T Get<T>(this ISession session, string key)
        {
            var value = session.Keys.Contains(key) ? session.GetString(key) : null;
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }

        public static void Set<T>(this ISession session, string key, T value)
        {
            session.SetString(key, JsonSerializer.Serialize<T>(value));
        }

        public static async Task SetAsync<T>(this ISession session, string key, T value)
        {
            if (!session.IsAvailable)
                await session.LoadAsync();
            session.SetString(key, JsonSerializer.Serialize(value));
        }

        public static async Task<T> GetAsync<T>(this ISession session, string key)
        {
            if (!session.IsAvailable)
                await session.LoadAsync();
            var value = session.GetString(key);
            return value == null ? default : JsonSerializer.Deserialize<T>(value);
        }

        public static void SetBoolean(this ISession session, string key, bool value)
        {
            session.Set(key, BitConverter.GetBytes(value));
        }

        public static bool? GetBoolean(this ISession session, string key)
        {
            var data = session.Get(key);
            if (data == null)
            {
                return null;
            }
            return BitConverter.ToBoolean(data, 0);
        }

        public static void SetDouble(this ISession session, string key, double value)
        {
            session.Set(key, BitConverter.GetBytes(value));
        }

        public static double? GetDouble(this ISession session, string key)
        {
            var data = session.Get(key);
            if (data == null)
            {
                return null;
            }
            return BitConverter.ToDouble(data, 0);
        }

        public static void SetInt64(this ISession session, string key, long value)
        {
            session.Set(key, BitConverter.GetBytes(value));
        }

        public static double? GetInt64(this ISession session, string key)
        {
            var data = session.Get(key);
            if (data == null) return null;
            return BitConverter.ToInt64(data, 0);
        }
    }
}
