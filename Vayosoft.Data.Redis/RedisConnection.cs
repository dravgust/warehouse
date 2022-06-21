using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Vayosoft.Data.Redis
{
    public class RedisConnection : IRedisDatabaseConnection, IRedisSubscriberConnection, IDisposable
    {
        private readonly Lazy<ConnectionMultiplexer> LazyConnection;
        
        [ActivatorUtilitiesConstructor]
        public RedisConnection(IConfiguration config)
            : this(ConfigurationOptions.Parse(config["Redis:Connection"])) { }
        public RedisConnection()
            : this(new ConfigurationOptions
            {
                AbortOnConnectFail = false,
                EndPoints = { "127.0.0.1:6379" }
            })
        { }
        public RedisConnection(string connectionString)
            : this(ConfigurationOptions.Parse(connectionString))
        { }
        public RedisConnection(ConfigurationOptions options)
        {
            LazyConnection = new Lazy<ConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(options));
        }

        public ConnectionMultiplexer Connection => LazyConnection.Value;

        public IDatabase Database => Connection.GetDatabase();
        public ISubscriber Subscriber => Connection.GetSubscriber();

        public void Dispose()
        {
            Connection.Dispose();
        }
    }
}
