using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Vayosoft.Redis;
using Vayosoft.Streaming.Redis.Consumers;
using Vayosoft.Streaming.Redis.Producers;

namespace Warehouse.IntegrationTests
{
    public class RedisFixture : IDisposable
    {
        public readonly RedisStreamFixtureConfiguration Options = new();
        private readonly IRedisProvider _dbProvider;
        public void Configure(Action<RedisStreamFixtureConfiguration> configure)
            => configure(Options);

        public IRedisDatabaseProvider Connection => _dbProvider;

        public RedisFixture()
        {
            _dbProvider = new RedisProvider(Options.ConnectionString);
        }

        public RedisProducer GetProducer(string topic)
        {
            return new RedisProducer(_dbProvider, new RedisProducerConfig { MaxLength = 10, Topic = topic });
        }

        public RedisConsumer GetConsumer()
        {
            var configBuilder = new ConfigurationBuilder();
            IConfiguration configuration = configBuilder.Build();
            var logger = Options.LoggerFactory.CreateLogger<RedisConsumer>();
            return new RedisConsumer(_dbProvider, configuration, logger);
        }

        public RedisConsumerGroup GetConsumerGroup()
        {
            var configBuilder = new ConfigurationBuilder()
                .AddInMemoryCollection(new Dictionary<string, string>
                {
                    {"RedisConsumer:GroupId", "TEST_GROUP"},
                    {"RedisConsumer:ConsumerId", "TEST_GROUP_0"}
                });
            IConfiguration configuration = configBuilder.Build();
            var logger = Options.LoggerFactory.CreateLogger<RedisConsumerGroup>();
            return new RedisConsumerGroup(_dbProvider, configuration, logger);
        }

        public void Dispose()
        {
            _dbProvider.Dispose();
        }
    }

    public class RedisStreamFixtureConfiguration
    {
        public string ConnectionString { get; set; } = "192.168.10.11:6379,abortConnect=false,ssl=false";
        public ILoggerFactory LoggerFactory { get; set; }

        public void Deconstruct(out string connectionString, out ILoggerFactory loggerFactory)
        {
            connectionString = ConnectionString;
            loggerFactory = LoggerFactory;
        }
    }
}
