using StackExchange.Redis;

namespace Vayosoft.Data.Redis
{
    public interface IRedisDatabaseProvider
    {
        public IDatabase Database { get; }
    }
}
