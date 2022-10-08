using System.Text.Json;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Vayosoft.Redis;
using Vayosoft.Streaming.Consumers;

namespace Vayosoft.Streaming.Redis.Consumers
{
    public sealed class RedisSubscriber : IRedisConsumer<ConsumeResult>
    {
        private readonly ISubscriber _subscriber;
        private readonly ILogger<RedisConsumerGroup> _logger;

        public RedisSubscriber(IRedisSubscriberProvider connection, ILogger<RedisConsumerGroup> logger)
        {
            _subscriber = connection.Subscriber;
            _logger = logger;
        }
        
        public ChannelReader<ConsumeResult> Subscribe(string[] topics, CancellationToken cancellationToken)
        {
            var channel = Channel.CreateUnbounded<ConsumeResult>();

            var redisChannels = new List<RedisChannel>();
            foreach (var topic in topics)
            {
                var redisChannel = new RedisChannel(topic, RedisChannel.PatternMode.Auto);

                _subscriber.Subscribe(redisChannel, (ch, message) =>
                {
                    _ = Producer(channel, topic, message, cancellationToken);
                });

                redisChannels.Add(redisChannel);
            }

            _logger.LogInformation("Subscribed to channels {Topics}", 
                string.Join(", ", topics));

            cancellationToken.Register(() =>
            {
                foreach (var ch in redisChannels)
                {
                    _subscriber.Unsubscribe(ch);
                }

                channel.Writer.Complete();

                _logger.LogInformation("Unsubscribed from channels {Topics}",
                    string.Join(", ", topics));
            });

            return channel.Reader;
        }

        private async Task Producer(ChannelWriter<ConsumeResult> writer, string streamName, string message, CancellationToken token)
        {
            if (string.IsNullOrEmpty(message)) return;
            try
            {
                var eventMessage = JsonSerializer.Deserialize<Message>(message);
                if (eventMessage is null) return;

                await writer.WriteAsync(new ConsumeResult(streamName, eventMessage.Key, eventMessage.Value), token);
            }
            catch (Exception e)
            {
                _logger.LogError("{Message}\r\n{StackTrace}", 
                    e.Message, e.StackTrace);
            }
        }
    }
}
