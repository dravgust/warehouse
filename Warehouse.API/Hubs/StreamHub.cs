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
                .Configure(options =>
                {
                    options.ConsumerId = Context.ConnectionId;
                    options.Interval = 1000;
                })
                .Subscribe(new []{ "IPS-EVENTS" }, cancellationToken);
        }

        //private ChannelReader<IEvent> GetEvents(ChannelReader<ConsumeResult<string, string>> reader, CancellationToken token)
        //{
        //    var channel = Channel.CreateUnbounded<IEvent>();

        //    return channel.Reader;
        //}
    }
}
