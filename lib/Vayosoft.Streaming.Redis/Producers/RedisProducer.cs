using System.Diagnostics.CodeAnalysis;
using System.Text.Json;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;
using Vayosoft.Core.SharedKernel.Events.External;
using Vayosoft.Redis;

namespace Vayosoft.Streaming.Redis.Producers
{
    public class RedisProducer : IExternalEventProducer
    {
        private readonly IDatabase _database;
        private readonly string _topic;
        private readonly int? _maxLength;

        [ActivatorUtilitiesConstructor]
        public RedisProducer(IRedisDatabaseProvider connection, IConfiguration configuration)
            : this(connection, configuration.GetRedisProducerConfig())
        { }

        public RedisProducer(IRedisDatabaseProvider connection,[NotNull] RedisProducerConfig config)
        {
            _topic = config.Topic ?? nameof(IExternalEvent);
            _maxLength = config.MaxLength > 0 ? config.MaxLength : null;

            _database = connection.Database;
        }

        public async Task Publish(IExternalEvent @event)
        {
            await Task.Yield();

            var type = @event.GetType();

            _ = await _database.StreamAddAsync(
                _topic,
                type.Name,
                JsonSerializer.Serialize(@event, type),
                useApproximateMaxLength: true,
                maxLength: _maxLength);
        }
    }
}
