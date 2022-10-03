using System.Runtime.CompilerServices;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using Vayosoft.Core.SharedKernel.Events;
using Vayosoft.Core.Utilities;
using Vayosoft.Data.Redis;

namespace Vayosoft.Streaming.Redis.Consumers
{
    public sealed class RedisConsumer : IRedisConsumer
    {
        private const int IntervalMilliseconds = 1000;

        private readonly ILogger<RedisConsumerGroup> _logger;
        private readonly RedisStreamConsumerConfig _config;
        private readonly IDatabase _database;

        public RedisConsumer(IRedisDatabaseProvider connection, ILogger<RedisConsumerGroup> logger)
        {
            _logger = logger;
            _config = new RedisStreamConsumerConfig();
            _database = connection.Database;
        }

        public ChannelReader<IEvent> Subscribe(string[] topics, CancellationToken cancellationToken)
        {
            var consumerName = _config?.ConsumerId ?? Guid.NewGuid().ToString();

            var channel = Channel.CreateUnbounded<IEvent>();

            foreach (var topic in topics)
            {
                _ = Producer(channel, topic, cancellationToken);

                _logger.LogInformation("[{ConsumerName}] Subscribed to stream {Topic}", consumerName, topic);
            }

            return channel.Reader;
        }

        public IRedisConsumer Configure(Action<RedisStreamConsumerConfig> configuration)
        {
            configuration(_config);
            return this;
        }

        private async Task Producer(ChannelWriter<IEvent> writer, string streamName, CancellationToken cancellationToken)
        {
            Exception localException = null;
            try
            {
                await foreach(var item in FetchItemsAsync(streamName, cancellationToken))
                {
                    await writer.WriteAsync(item, cancellationToken);
                }
            }
            catch (TaskCanceledException) { }
            catch (Exception e)
            {
                localException = e;
            }
            finally
            {
                writer.Complete(localException);
            }
        }

        private async IAsyncEnumerable<IEvent> FetchItemsAsync(string streamName, [EnumeratorCancellation] CancellationToken token)
        {
            var streamInfo = await _database.StreamInfoAsync(streamName);
            var lastGeneratedId = streamInfo.LastGeneratedId;

            while (!token.IsCancellationRequested)
            {
                var streamEntries = await _database.StreamReadAsync(streamName, lastGeneratedId);
                if (!streamEntries.Any())
                    await Task.Delay(IntervalMilliseconds, token);
                else
                {
                    foreach (var streamEntry in streamEntries)
                    {
                        foreach (var nameValueEntry in streamEntry.Values)
                        {
                            var eventType = TypeProvider.GetTypeFromAnyReferencingAssembly(nameValueEntry.Name);
                            var @event = JsonConvert.DeserializeObject(nameValueEntry.Value, eventType);

                            yield return (IEvent)@event;
                        }

                        lastGeneratedId = streamEntry.Id;
                    }

                }
            }
        }
    }
}
