using System.Threading.Channels;

namespace Vayosoft.Streaming.Consumers
{
    public interface IChannelConsumer<T> : IConsumer<ChannelReader<T>> { }
}
