using System.Reactive;
using System.Text.Json;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using StackExchange.Redis;
using Vayosoft.Data.Redis;

namespace Vayosoft.Streaming.Redis.Consumers
{
    public class RedisSubscriber : IRedisConsumer<ConsumeResult>
    {
        private readonly ISubscriber _subscriber;
        private readonly ILogger<RedisConsumerGroup> _logger;

        public RedisSubscriber(IRedisSubscriberProvider connection, ILogger<RedisConsumerGroup> logger)
        {
            _subscriber = connection.Subscriber;

            this._logger = logger;
        }

        public void Subscribe(string[] topics, Action<ConsumeResult> action, CancellationToken cancellationToken)
        {
            foreach (var topic in topics)
            {
                var observer = new AnonymousObserver<ConsumeResult>(
                onNext: action,
                onCompleted: () => _logger.LogInformation("Unsubscribed from channel {Topic}", topic),
                onError: (e) => _logger.LogError("{Message}\r\n{StackTrace}", e.Message, e.StackTrace));

                _subscriber.Subscribe(new RedisChannel(topic, RedisChannel.PatternMode.Auto), (channel, message) =>
                {
                    if (string.IsNullOrEmpty(message)) return;
                    try
                    {
                        var eventMessage = JsonSerializer.Deserialize<Message<string, string>>(message);
                        if (eventMessage == null) return;
                        observer.OnNext(ConsumeResult.Create(topic, eventMessage.Key, eventMessage.Value));
                    }
                    catch (Exception ex) { observer.OnError(ex); }
                });

                _logger.LogInformation("Subscribed to channel {Topic}", topic);
            }
        }

        public void Close()
        {
            _subscriber.UnsubscribeAll();
        }

        public ChannelReader<ConsumeResult> Subscribe(string[] topics, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
