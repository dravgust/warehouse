using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Vayosoft.Data.Redis
{
    public class RedisProvider : IRedisConnectionProvider, IRedisDatabaseProvider, IRedisSubscriberProvider, IDisposable
    {
        private readonly Lazy<IConnectionMultiplexer> LazyConnection;
        
        [ActivatorUtilitiesConstructor]
        public RedisProvider(IConfiguration config)
            : this(ConfigurationOptions.Parse(config["ConnectionStrings:RedisConnectionString"])) { }
        public RedisProvider()
            : this(new ConfigurationOptions
            {
                AbortOnConnectFail = false,
                EndPoints = { "127.0.0.1:6379" }
            })
        { }
        public RedisProvider(string connectionString)
            : this(ConfigurationOptions.Parse(connectionString))
        { }
        public RedisProvider(ConfigurationOptions options)
        {
            LazyConnection = new Lazy<IConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(options));
        }

        public IConnectionMultiplexer Connection => LazyConnection.Value;

        public IDatabase Database => Connection.GetDatabase();
        public ISubscriber Subscriber => Connection.GetSubscriber();

        public void Dispose()
        {
            Connection.Dispose();
        }
    }
}
