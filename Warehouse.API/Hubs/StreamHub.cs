using System.Text.Json;
using System.Threading.Channels;
using App.Metrics;
using Microsoft.AspNetCore.SignalR;
using Vayosoft.Utilities;
using Vayosoft.Streaming.Consumers;
using Vayosoft.Streaming.Redis.Consumers;
using Warehouse.API.Diagnostic;
using Warehouse.API.Services.Authorization;
using Warehouse.Core.Application.Common.Persistence;
using Warehouse.Core.Application.Common.Services.Security;
using Warehouse.Core.Domain.Events;
using Warehouse.Core.Domain.ValueObjects;

namespace Warehouse.API.Hubs
{
    [PermissionAuthorization]
    public sealed class StreamHub : Hub
    {
        private readonly RedisConsumer _consumer;
        private readonly IWarehouseStore _store;
        private readonly IMetrics _metrics;

        public StreamHub(RedisConsumer consumer, IWarehouseStore store, IMetrics metrics)
        {
            _consumer = consumer;
            _store = store;
            _metrics = metrics;
        }

        public ChannelReader<Notification> Notifications(CancellationToken cancellationToken)
        {
            var providerId = Context.User?.Identity.GetProviderId();

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

        public override Task OnConnectedAsync()
        {
            _metrics.Measure.Counter.Increment(MetricsRegistry.ActiveUserCounter);
            return base.OnConnectedAsync();
        }

        public override Task OnDisconnectedAsync(Exception exception)
        {
            _metrics.Measure.Counter.Decrement(MetricsRegistry.ActiveUserCounter);
            return base.OnDisconnectedAsync(exception);
        }
    }
}
