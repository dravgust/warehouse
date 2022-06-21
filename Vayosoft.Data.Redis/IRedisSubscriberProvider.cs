using StackExchange.Redis;

namespace Vayosoft.Data.Redis
{
    public interface IRedisSubscriberProvider
    {
        public ISubscriber Subscriber { get; }
    }
}
