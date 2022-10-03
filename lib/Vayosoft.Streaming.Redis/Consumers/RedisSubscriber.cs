using System.Reactive;
using System.Threading.Channels;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using StackExchange.Redis;
using Vayosoft.Core.SharedKernel.Events;
using Vayosoft.Data.Redis;

namespace Vayosoft.Streaming.Redis.Consumers
{
    public class RedisSubscriber : IRedisConsumer
    {
        private readonly ISubscriber _subscriber;
        private readonly ILogger<RedisConsumerGroup> _logger;

        public RedisSubscriber(IRedisSubscriberProvider connection, ILogger<RedisConsumerGroup> logger)
        {
            _subscriber = connection.Subscriber;

            this._logger = logger;
        }

        public void Subscribe(string[] topics, Action<ConsumeResult<string, string>> action, CancellationToken cancellationToken)
        {
            foreach (var topic in topics)
            {
                var observer = new AnonymousObserver<ConsumeResult<string, string>>(
                onNext: action,
                onCompleted: () => _logger.LogInformation("Unsubscribed from channel {Topic}", topic),
                onError: (e) => _logger.LogError("{Message}\r\n{StackTrace}", e.Message, e.StackTrace));

                _subscriber.Subscribe(new RedisChannel(topic, RedisChannel.PatternMode.Auto), (channel, message) =>
                {
                    if (string.IsNullOrEmpty(message)) return;
                    try
                    {
                        var eventMessage = JsonConvert.DeserializeObject<Message<string, string>>(message);
                        if (eventMessage == null) return;
                        observer.OnNext(new ConsumeResult<string, string>(topic, eventMessage.Key, eventMessage.Value));
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

        public ChannelReader<IEvent> Subscribe(string[] topics, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
