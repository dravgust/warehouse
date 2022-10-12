using System.Text.Json;
using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR;
using Vayosoft.Core.Utilities;
using Vayosoft.Streaming.Consumers;
using Vayosoft.Streaming.Redis.Consumers;
using Warehouse.Core.Application.Common.Persistence;
using Warehouse.Core.Domain.Entities;
using Warehouse.Core.Domain.Events;

namespace Warehouse.API.Hubs
{
    public sealed class StreamHub : Hub
    {
        private readonly RedisConsumer _consumer;
        private readonly IWarehouseStore _store;

        public StreamHub(RedisConsumer consumer, IWarehouseStore store)
        {
            _consumer = consumer;
            _store = store;
        }

        public ChannelReader<Notification> Notifications(CancellationToken cancellationToken)
        {
            var eventStream = _consumer
                .Configure(options =>
                {
                    options.ConsumerId = Context.ConnectionId;
                    options.Interval = 1000;
                })
                .Subscribe(new []{ "IPS-EVENTS" }, cancellationToken);

            return GetEvents(eventStream, cancellationToken);
        }

        private ChannelReader<Notification> GetEvents(ChannelReader<ConsumeResult> reader, CancellationToken token)
        {
            var channel = Channel.CreateUnbounded<Notification>();

            async Task Consumer()
            {
                await foreach (var result in reader.ReadAllAsync(token))
                {
                    var notification = await GetNotification(result, token);
                    await channel.Writer.WriteAsync(notification, token);
                }
            }

            _ = Consumer();

            return channel.Reader;
        }

        private async Task<Notification> GetNotification(ConsumeResult result, CancellationToken cancellationToken)
        {
            var eventType = TypeProvider.GetTypeFromAnyReferencingAssembly(result.Message.Key);
            var @event = JsonSerializer.Deserialize(result.Message.Value, eventType);

            return @event switch
            {
                TrackedItemMoved e => new Notification(e.Timestamp)
                {
                    Message = $"'{await GetTrackedItemName(e.Id, cancellationToken)}'" +
                              $" moved from '{await GetSiteName(e.SourceId, cancellationToken)}'" +
                              $" to '{await GetSiteName(e.DestinationId, cancellationToken)}'"
                },
                TrackedItemEntered e => new Notification(e.Timestamp)
                {
                    Message = $"'{await GetTrackedItemName(e.Id, cancellationToken)}'" +
                              $" entered '{await GetSiteName(e.DestinationId, cancellationToken)}'"
                },
                TrackedItemGotOut e => new Notification(e.Timestamp)
                {
                    Message = $"'{await GetTrackedItemName(e.Id, cancellationToken)}'" +
                              $" out of '{await GetSiteName(e.SourceId, cancellationToken)}'"
                },
                _ => new Notification(DateTime.UtcNow)
            };
        }

        private async Task<string> GetSiteName(string siteId, CancellationToken token)
        {
            return (await _store.Sites.FindAsync(siteId, token))?.Name;
        }

        private async Task<string> GetTrackedItemName(string id, CancellationToken token)
        {
            return (await _store.TrackedItems.FirstOrDefaultAsync(q => q.Id.Equals(id), token))?.Name ?? id;
        }
    }
}
