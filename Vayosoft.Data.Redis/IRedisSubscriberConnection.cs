using StackExchange.Redis;

namespace Vayosoft.Data.Redis
{
    public interface IRedisSubscriberConnection
    {
        public ISubscriber Subscriber { get; }
    }
}
