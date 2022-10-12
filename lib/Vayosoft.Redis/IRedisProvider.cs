using System;
using StackExchange.Redis;

namespace Vayosoft.Redis
{
    public interface IRedisProvider : IRedisConnectionProvider, IRedisDatabaseProvider, IRedisSubscriberProvider
    { }

    public interface IRedisConnectionProvider : IDisposable
{
        public IConnectionMultiplexer Connection { get; }
    }

    public interface IRedisDatabaseProvider
{
        public IDatabase Database { get; }
    }

    public interface IRedisSubscriberProvider
{
        public ISubscriber Subscriber { get; }
    }
}
