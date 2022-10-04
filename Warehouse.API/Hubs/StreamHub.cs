using System.Text.Json;
using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR;
using Vayosoft.Core.SharedKernel.Events;
using Vayosoft.Core.Utilities;
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
            var stream = _consumer
                .Configure(options =>
                {
                    options.ConsumerId = Context.ConnectionId;
                    options.Interval = 1000;
                })
                .Subscribe(new []{ "IPS-EVENTS" }, cancellationToken);

            return GetEvents(stream, cancellationToken);
        }

        private static ChannelReader<IEvent> GetEvents(ChannelReader<ConsumeResult> reader, CancellationToken token)
        {
            var channel = Channel.CreateUnbounded<IEvent>();

            async Task Consumer()
            {
                await foreach (var result in reader.ReadAllAsync(token))
                {
                    var message = result.Message;
                    var eventType = TypeProvider.GetTypeFromAnyReferencingAssembly(message.Key);
                    var @event = JsonSerializer.Deserialize(message.Value, eventType);

                    await channel.Writer.WriteAsync((IEvent) @event, token);
                }
            }

            _ = Consumer();

            return channel.Reader;
        }
    }
}
