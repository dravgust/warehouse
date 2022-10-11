using System.Text;
using System.Text.Json;
using System.Threading;
using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR;
using Vayosoft.Core.SharedKernel.Events;
using Vayosoft.Core.Utilities;
using Vayosoft.Streaming.Consumers;
using Vayosoft.Streaming.Redis.Consumers;
using Warehouse.Core.Application.Persistence;
using Warehouse.Core.Application.UseCases.BeaconTracking.Models;
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

            TrackedItem item; 
            switch (@event)
            {
                case TrackedItemMoved e:

                    item = await _store.TrackedItems.FirstOrDefaultAsync(q => q.Id.Equals(e.Id), cancellationToken);
                    return new Notification(DateTime.UtcNow)
                    {
                        Message = $"'{item.Name}' moved from '{await GetSiteName(e.SourceId, cancellationToken)}' to '{await GetSiteName(e.DestinationId, cancellationToken)}'"
                    };
                case TrackedItemEntered e:
                    item = await _store.TrackedItems.FirstOrDefaultAsync(q => q.Id.Equals(e.Id), cancellationToken);
                    return new Notification(DateTime.UtcNow)
                    {
                        Message = $"'{item.Name}' entered '{await GetSiteName(e.DestinationId, cancellationToken)}'"
                    };
                case TrackedItemGotOut e:
                    item = await _store.TrackedItems.FirstOrDefaultAsync(q => q.Id.Equals(e.Id), cancellationToken);
                    return new Notification(DateTime.UtcNow)
                    {
                        Message = $"'{item.Name}' out of '{await GetSiteName(e.SourceId, cancellationToken)}'"
                    };
                default:
                    return new Notification(DateTime.UtcNow);
            }
        }

        private async Task<string> GetSiteName(string siteId, CancellationToken token)
        {
            return (await _store.Sites.FindAsync(siteId, token))?.Name;
        }
    }

    public record Notification(DateTime TimeStamp)
    {
        public string Message { get; init; }
    };
}
