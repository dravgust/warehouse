using System.Threading.Channels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using Vayosoft.Core.SharedKernel.Events;
using Vayosoft.Core.Utilities;
using Vayosoft.Data.Redis;

namespace Vayosoft.Streaming.Redis.Consumers
{
    public sealed class RedisConsumerGroup : IRedisConsumer
    {
        private const int IntervalMilliseconds = 300;

        private readonly ILogger<RedisConsumerGroup> _logger;
        private readonly RedisStreamConsumerConfig _config;
        private readonly IDatabase _database;
  
        public RedisConsumerGroup(IRedisDatabaseProvider connection, IConfiguration configuration, ILogger<RedisConsumerGroup> logger)
        {
            Guard.NotNull(configuration);
            _config = configuration.GetRedisConsumerConfig();

            _logger = logger;
            _database = connection.Database;
        }

        public ChannelReader<IEvent> Subscribe(string[] topics, CancellationToken cancellationToken)
        {
            var consumerName = _config?.ConsumerId ?? Guid.NewGuid().ToString();
            var groupName = _config?.GroupId ?? consumerName;

            var channel = Channel.CreateUnbounded<IEvent>();

            foreach (var topic in topics)
            {
                _ = Producer(channel, topic, groupName, consumerName, cancellationToken);

                _logger.LogInformation("[{GroupName}.{ConsumerName}] Subscribed to stream {Topic}", groupName, consumerName, topic);
            }

            return channel.Reader;
        }

        public IRedisConsumer Configure(Action<RedisStreamConsumerConfig> configuration)
        {
            configuration(_config);
            return this;
        }

        private async Task Producer(ChannelWriter<IEvent> writer, string streamName, string groupName, string consumerName, CancellationToken token)
        {
            var streamInfo = await _database.StreamInfoAsync(streamName);
            var lastGeneratedId = streamInfo.LastGeneratedId;

            if (!(await _database.KeyExistsAsync(streamName)) ||
                (await _database.StreamGroupInfoAsync(streamName)).All(x => x.Name != groupName))
            {
                await _database.StreamCreateConsumerGroupAsync(streamName, groupName, lastGeneratedId);
            }

            Exception localException = null;
            try
            {
                while (!token.IsCancellationRequested)
                {
                    var streamEntries = await _database.StreamReadGroupAsync(streamName, groupName, consumerName, ">", 1);
                    if (!streamEntries.Any())
                        await Task.Delay(IntervalMilliseconds, token);
                    else
                    {
                        var streamEntry = streamEntries.Last();
                        foreach (var nameValueEntry in streamEntry.Values)
                        {
                            try
                            {
                                var eventType = TypeProvider.GetTypeFromAnyReferencingAssembly(nameValueEntry.Name);
                                var @event = JsonConvert.DeserializeObject(nameValueEntry.Value, eventType);

                                await writer.WriteAsync((IEvent)@event, token);
                                await _database.StreamAcknowledgeAsync(streamName, groupName, streamEntry.Id);
                            }
                            catch (Exception e)
                            {
                                _logger.LogError("{Message}\r\n{StackTrace}", e.Message, e.StackTrace);
                            }
                        }
                    }
                }
            }
            catch (OperationCanceledException) { /*ignore*/ }
            catch (Exception e)
            {
                localException = e;
            }
            finally
            {
                _database.StreamDeleteConsumer(streamName, groupName, consumerName);
                writer.Complete(localException);
            }
        }
    }
}
