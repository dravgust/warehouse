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
    }
}
