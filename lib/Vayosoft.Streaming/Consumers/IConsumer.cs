using System.Threading.Channels;

namespace Vayosoft.Streaming.Consumers
{
    public interface IConsumer<T>
    {
        public ChannelReader<T> Subscribe(string[] topics, CancellationToken cancellationToken);
    }
}
