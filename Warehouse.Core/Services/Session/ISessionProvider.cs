namespace Warehouse.Core.Services.Session
{
    public interface ISessionProvider
    {
        byte[] this[string index]
        {
            get;
            set;
        }

        public T Get<T>(string key);

        public void Set<T>(string key, T value);

        public void SetBoolean(string key, bool value);

        public bool? GetBoolean(string key);

        public void SetDouble(string key, double value);

        public double? GetDouble(string key);

        public void SetInt64(string key, long value);

        public double? GetInt64(string key);
    }
}
