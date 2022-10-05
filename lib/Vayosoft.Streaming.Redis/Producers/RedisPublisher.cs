using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using System.Text.Json;
using Vayosoft.Core.SharedKernel.Events.External;
using Vayosoft.Redis;

namespace Vayosoft.Streaming.Redis.Producers
{
    public class RedisPublisher
    {
        private readonly ISubscriber _subscriber;
        private readonly RedisProducerConfig _config;

        [ActivatorUtilitiesConstructor]
        public RedisPublisher(IRedisSubscriberProvider connection, IConfiguration configuration)
        {
            if (configuration == null) 
                throw new ArgumentNullException(nameof(configuration));
            _config = configuration.GetRedisProducerConfig();

            _subscriber = connection.Subscriber;
        }

        public RedisPublisher(IRedisSubscriberProvider connection, RedisProducerConfig config)
        {
            _config = config;
            _subscriber = connection.Subscriber;
        }

        public async Task Publish(IExternalEvent @event)
        {
            var topic = _config.Topic ?? nameof(IExternalEvent);

            var message = new Message(@event.GetType().Name, JsonSerializer.Serialize(@event));

            await Task.Yield();
            var result = await _subscriber.PublishAsync(
                topic, JsonSerializer.Serialize(message));
        }
    }
}
