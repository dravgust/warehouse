﻿using System.Threading.Channels;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Vayosoft.Core.Utilities;
using Vayosoft.Data.Redis;

namespace Vayosoft.Streaming.Redis.Consumers
{
    public sealed class RedisConsumer : IRedisConsumer<ConsumeResult>
    {
        private readonly ILogger<RedisConsumerGroup> _logger;
        private readonly RedisStreamConsumerConfig _config;
        private readonly IDatabase _database;

        public RedisConsumer(IRedisDatabaseProvider connection, IConfiguration configuration, ILogger<RedisConsumerGroup> logger)
        {
            Guard.NotNull(configuration);
            _config = configuration.GetRedisConsumerConfig();

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

                _logger.LogInformation("[{ConsumerName}] Subscribed to stream {Topic}.", consumerName, topic);
            }

            return channel.Reader;
        }

        public IRedisConsumer<ConsumeResult> Configure(Action<RedisStreamConsumerConfig> options)
        {
            options(_config);
            return this;
        }

        private async Task Producer(ChannelWriter<ConsumeResult> writer, string streamName, CancellationToken token)
        {
            Exception localException = null;
            try
            {
                var streamInfo = await _database.StreamInfoAsync(streamName);
                var lastGeneratedId = streamInfo.LastGeneratedId;
                var intervalMilliseconds = _config.Interval;

                while (!token.IsCancellationRequested)
                {
                    var streamEntries = await _database.StreamReadAsync(streamName, lastGeneratedId);
                    if (!streamEntries.Any())
                        await Task.Delay(intervalMilliseconds, token);
                    else
                    {

                        foreach (var streamEntry in streamEntries)
                        {
                            foreach (var nameValueEntry in streamEntry.Values)
                            {
                                try
                                {
                                    await writer.WriteAsync(
                                        ConsumeResult.Create(streamName, nameValueEntry.Name,
                                            nameValueEntry.Value), token);
                                }
                                catch (Exception e)
                                {
                                    _logger.LogError("{Message}\r\n{StackTrace}", e.Message, e.StackTrace);
                                }
                            }

                            lastGeneratedId = streamEntry.Id;
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
