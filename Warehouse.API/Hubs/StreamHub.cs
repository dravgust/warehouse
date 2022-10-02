using System.Threading.Channels;
using Microsoft.AspNetCore.SignalR;
using Newtonsoft.Json;
using StackExchange.Redis;
using Vayosoft.Core.SharedKernel.Events;
using Vayosoft.Core.Utilities;
using Vayosoft.Data.Redis;

namespace Warehouse.API.Hubs
{
    public sealed class StreamHub : Hub
    {
        private readonly IDatabase _redisDb;
        private readonly ILogger<StreamHub> _logger;

        public StreamHub(IRedisDatabaseProvider connection, ILogger<StreamHub> logger)
        {
            _redisDb = connection.Database;
            _logger = logger;
        }

        public ChannelReader<IEvent> Notifications(CancellationToken cancellationToken)
        {
            var channel = Channel.CreateUnbounded<IEvent>();

            _ = WriteItemsAsync(channel, cancellationToken);

            return channel.Reader;
        }

        private async Task WriteItemsAsync(ChannelWriter<IEvent> writer,
            CancellationToken cancellationToken)
        {
            Exception localException = null;
            try
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
                            try
                            {
                                foreach (var nameValueEntry in streamEntry.Values)
                                {
                                    var eventType = TypeProvider.GetTypeFromAnyReferencingAssembly(nameValueEntry.Name);
                                    var @event = JsonConvert.DeserializeObject(nameValueEntry.Value, eventType);

                                    _logger.LogInformation("STREAM_EVENT: {Event}", @event.ToJson());
                                    await writer.WriteAsync((IEvent) @event, cancellationToken);
                                }

                                lastReadMessage = streamEntry.Id;
                            }
                            catch (Exception e)
                            {
                                _logger.LogError(e.Message);
                            }
                        }

                    }
                }
            }
            catch(TaskCanceledException){}
            catch (Exception e)
            {
                _logger.LogError(e.Message);
                localException = e;
            }
            finally
            {
                writer.Complete(localException);
            }
        }
    }
}
