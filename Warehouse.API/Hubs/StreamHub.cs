using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR;
using Vayosoft.Core.SharedKernel.Events;
using Vayosoft.Streaming.Redis.Consumers;

namespace Warehouse.API.Hubs
{
    public sealed class StreamHub : Hub
    {
        private readonly RedisConsumer _consumer;

        public StreamHub(RedisConsumer consumer)
        {
            _consumer = consumer;
        }

        public ChannelReader<IEvent> Notifications(CancellationToken cancellationToken)
        {
            return _consumer
                .Configure(options => options.ConsumerId = Context.ConnectionId)
                .Subscribe(new []{ "IPS-EVENTS" }, cancellationToken);
        }
    }
}
