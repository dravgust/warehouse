using System.Runtime.CompilerServices;
using Newtonsoft.Json;
using StackExchange.Redis;
using Vayosoft.Core.SharedKernel.Events;
using Vayosoft.Core.Utilities;
using Warehouse.API.Hubs;

namespace Warehouse.API.Services
{
    public class EventLocalChannel : LocalChannelBase<IEvent>
    {
        private readonly IDatabase _redisDb;
        private readonly ILogger<StreamHub> _logger;

        public EventLocalChannel(IDatabase redisDb, ILogger<StreamHub> logger)
        {
            _redisDb = redisDb;
            _logger = logger;
        }

        public override async IAsyncEnumerable<IEvent> FetchItemsAsync([EnumeratorCancellation] CancellationToken cancellationToken)
        {
            var lastReadMessage = "0-0";
            var streamEntries = await _redisDb.StreamReadAsync("IPS-EVENTS", lastReadMessage);
            if (streamEntries.Any())
            {
                lastReadMessage = streamEntries.Last().Id;
            }
            while (!cancellationToken.IsCancellationRequested)
            {
                streamEntries = await _redisDb.StreamReadAsync("IPS-EVENTS", lastReadMessage);
                if (!streamEntries.Any())
                    await Task.Delay(1000, cancellationToken);
                else
                {
                    foreach (var streamEntry in streamEntries)
                    {
                        foreach (var nameValueEntry in streamEntry.Values)
                        {
                            var eventType = TypeProvider.GetTypeFromAnyReferencingAssembly(nameValueEntry.Name);
                            var @event = JsonConvert.DeserializeObject(nameValueEntry.Value, eventType);

                            _logger.LogInformation("STREAM_EVENT: {Event}", @event.ToJson());
                            yield return (IEvent) @event;
                        }

                        lastReadMessage = streamEntry.Id;
                    }

                }
            }
        }
    }
}
