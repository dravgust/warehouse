using System.Diagnostics.CodeAnalysis;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using StackExchange.Redis;
using Vayosoft.Core.SharedKernel.Events.External;
using Vayosoft.Data.Redis;

namespace Vayosoft.Streaming.Redis.Producers
{
    public class RedisProducer : IExternalEventProducer
    {
        private readonly IDatabase _database;
        private readonly RedisProducerConfig _config;

        [ActivatorUtilitiesConstructor]
        public RedisProducer(IRedisDatabaseProvider connection, IConfiguration configuration)
        {
            if (configuration == null) 
                throw new ArgumentNullException(nameof(configuration));
            _config = configuration.GetRedisProducerConfig();

            _database = connection.Database;
        }

        public RedisProducer(IRedisDatabaseProvider connection,[NotNull] RedisProducerConfig config)
        {
            _config = config;
            _database = connection.Database;
        }

        public async Task Publish(IExternalEvent @event)
        {
            var topic = _config.Topic ?? nameof(IExternalEvent);
            int? maxLength = _config.MaxLength > 0 ? _config.MaxLength : null;

            await Task.Yield();
            var result = await _database.StreamAddAsync(
                topic,
                @event.GetType().Name,
                JsonConvert.SerializeObject(@event),
                useApproximateMaxLength: maxLength != null,
                maxLength: maxLength);
        }
    }
}
