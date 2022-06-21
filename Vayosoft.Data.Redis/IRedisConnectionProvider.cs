using StackExchange.Redis;

namespace Vayosoft.Data.Redis
{
    public interface IRedisConnectionProvider
    {
        public IConnectionMultiplexer Connection { get; }
    }
}
