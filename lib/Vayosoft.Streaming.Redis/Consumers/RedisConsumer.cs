using System.Threading.Channels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Vayosoft.Redis;
using Vayosoft.Streaming.Consumers;

namespace Vayosoft.Streaming.Redis.Consumers
{
    public sealed class RedisConsumer : IChannelConsumer<ConsumeResult>
    {
        private readonly ILogger<RedisConsumer> _logger;
        private readonly RedisStreamConsumerConfig _config;
        private readonly IDatabase _database;

        public RedisConsumer(
            IRedisDatabaseProvider connection, 
            IConfiguration configuration,
            ILogger<RedisConsumer> logger)
        {
            _config = configuration.GetRedisConsumerConfig()
                      ?? new RedisStreamConsumerConfig();
            
            _logger = logger;
            _database = connection.Database;
        }

        public ChannelReader<ConsumeResult> Subscribe(string[] topics, CancellationToken cancellationToken)
        {
            var consumerName = _config?.ConsumerId ?? Guid.NewGuid().ToString();

            var channel = Channel.CreateUnbounded<ConsumeResult>();

            foreach (var topic in topics)
            {
                _ = Producer(channel, topic, cancellationToken);

                _logger.LogInformation("[{ConsumerName}] Subscribed to stream {Topic}.",
                    consumerName, topic);
            }

            return channel.Reader;
        }

        public IChannelConsumer<ConsumeResult> Configure(Action<RedisStreamConsumerConfig> options)
        {
            options(_config);
            return this;
        }

        private async Task Producer(
            ChannelWriter<ConsumeResult> writer,
            string topic, 
            CancellationToken token)
        {
            Exception localException = null;
            try
            {
                var streamInfo = await _database.StreamInfoAsync(topic);
                var lastGeneratedId = streamInfo.LastGeneratedId;
                var interval = _config.Interval;

                while (!token.IsCancellationRequested)
                {
                    var entries = await _database.StreamReadAsync(topic, lastGeneratedId);
                    if (!entries.Any())
                        await Task.Delay(interval, token);
                    else
                    {
                        foreach (var entry in entries)
                        {
                            foreach (var valueEntry in entry.Values)
                            {
                                try
                                {
                                    await writer.WriteAsync(new ConsumeResult(topic, valueEntry.Name,
                                            valueEntry.Value), token);
                                }
                                catch (Exception e)
                                {
                                    _logger.LogError("{Message}\r\n{StackTrace}",
                                        e.Message, e.StackTrace);
                                }
                            }

                            lastGeneratedId = entry.Id;
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
                writer.Complete(localException);
            }
        }
    }
}
