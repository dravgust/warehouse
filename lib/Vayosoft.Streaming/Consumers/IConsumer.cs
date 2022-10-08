using System.Threading.Channels;

namespace Vayosoft.Streaming.Consumers
{
    public interface IConsumer<out TResult>
    {
        public TResult Subscribe(string[] topics, CancellationToken cancellationToken);
    }
}
