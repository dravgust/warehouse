using StackExchange.Redis;

namespace Vayosoft.Data.Redis
{
    public interface IRedisDatabaseConnection
    {
        public IDatabase Database { get; }
    }
}
