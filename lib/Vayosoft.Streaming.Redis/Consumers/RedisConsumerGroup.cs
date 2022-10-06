using System.Threading.Channels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Vayosoft.Redis;

namespace Vayosoft.Streaming.Redis.Consumers
{
    public sealed class RedisConsumerGroup : IRedisConsumer<ConsumeResult>
    {
        private readonly ILogger<RedisConsumerGroup> _logger;
        private readonly RedisStreamConsumerConfig _config;
        private readonly IDatabase _database;
  
        public RedisConsumerGroup(
            IRedisDatabaseProvider connection,
            IConfiguration configuration,
            ILogger<RedisConsumerGroup> logger)
        {
            _config = configuration.GetRedisConsumerConfig() 
                      ?? new RedisStreamConsumerConfig();
 
            _logger = logger;
            _database = connection.Database;
        }

        public ChannelReader<ConsumeResult> Subscribe(string[] topics, CancellationToken cancellationToken)
        {
            var consumerName = _config?.ConsumerId ?? Guid.NewGuid().ToString();
            var groupName = _config?.GroupId ?? consumerName;

            var channel = Channel.CreateUnbounded<ConsumeResult>();

            foreach (var topic in topics)
            {
                _ = Producer(channel, topic, groupName, consumerName, cancellationToken);

                _logger.LogInformation("[{GroupName}.{ConsumerName}] Subscribed to stream {Topic}.",
                    groupName, consumerName, topic);
            }

            return channel.Reader;
        }

        public IRedisConsumer<ConsumeResult> Configure(Action<RedisStreamConsumerConfig> configuration)
        {
            configuration(_config);
            return this;
        }

        private async Task Producer(
            ChannelWriter<ConsumeResult> writer, 
            string topic, 
            string groupName, 
            string consumerName,
            CancellationToken token)
        {
            Exception localException = null;
            try
            {
                var streamInfo = await _database.StreamInfoAsync(topic);
                var lastGeneratedId = streamInfo.LastGeneratedId;
                var interval = _config.Interval;

                if (!(await _database.KeyExistsAsync(topic)) ||
                    (await _database.StreamGroupInfoAsync(topic)).All(x => x.Name != groupName))
                {
                    await _database.StreamCreateConsumerGroupAsync(topic, groupName, lastGeneratedId);
                }

                while (!token.IsCancellationRequested)
                {
                    var entries = await _database.StreamReadGroupAsync(topic, groupName, consumerName, ">", 1);
                    if (!entries.Any())
                        await Task.Delay(interval, token);
                    else
                    {
                        var entry = entries.Last();
                        foreach (var valueEntry in entry.Values)
                        {
                            try
                            {
                                await writer.WriteAsync(new ConsumeResult(topic, valueEntry.Name, valueEntry.Value), token);
                                await _database.StreamAcknowledgeAsync(topic, groupName, entry.Id);
                            }
                            catch (Exception e)
                            {
                                _logger.LogError("{Message}\r\n{StackTrace}",
                                    e.Message, e.StackTrace);
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
                _database.StreamDeleteConsumer(topic, groupName, consumerName);
                writer.Complete(localException);
            }
        }
    }
}
