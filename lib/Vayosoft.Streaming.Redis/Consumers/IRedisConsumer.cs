using Vayosoft.Core.SharedKernel.Events;

namespace Vayosoft.Streaming.Redis.Consumers
{
    public interface IRedisConsumer : IConsumer<IEvent> { }
}
