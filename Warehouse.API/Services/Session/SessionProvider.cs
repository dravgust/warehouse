using System.Text.Json;
using Warehouse.API.Extensions;
using Warehouse.Core.Services.Session;

namespace Warehouse.API.Services.Session
{
    public class SessionProvider : ISessionProvider
    {
        private readonly IHttpContextAccessor _httpContextAccessor;

        protected ISession Session => _httpContextAccessor.HttpContext?.Session;

        public SessionProvider(IHttpContextAccessor httpContextAccessor)
        {
            _httpContextAccessor = httpContextAccessor;
        }

        public byte[] this[string key]
        {
            get => Session?.Get(key);
            set => Session?.Set(key, value);
        }

        public T Get<T>(string key) =>
            Session != null ? Session.Get<T>(key) : default;

        public void Set<T>(string key, T value) =>
            Session?.Set(key, value);

        public Task<T> GetAsync<T>(string key) =>
            Session?.GetAsync<T>(key);

        public Task SetAsync<T>(string key, T value) =>
            Session?.SetAsync(key, value);

        public void SetBoolean( string key, bool value) =>
            Session?.Set(key, BitConverter.GetBytes(value));

        public bool? GetBoolean(string key)
        {
            var data = Session?.Get(key);
            if (data == null) return null;
            return BitConverter.ToBoolean(data, 0);
        }

        public void SetDouble(string key, double value)
        {
            Session?.Set(key, BitConverter.GetBytes(value));
        }

        public double? GetDouble(string key)
        {
            var data = Session?.Get(key);
            if (data == null) return null;
            return BitConverter.ToDouble(data, 0);
        }

        public void SetInt64(string key, long value)
        {
            Session?.Set(key, BitConverter.GetBytes(value));
        }

        public double? GetInt64(string key)
        {
            var data = Session?.Get(key);
            if (data == null) return null;
            return BitConverter.ToInt64(data, 0);
        }

        public byte[] ToByteArray<T>(T obj) =>
            obj == null ? null : JsonSerializer.SerializeToUtf8Bytes(obj);

        public T FromByteArray<T>(byte[] data) =>
            data == null ? default : JsonSerializer.Deserialize<T>(data);

    }
}
