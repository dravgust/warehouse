using System;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using StackExchange.Redis;

namespace Vayosoft.Redis
{
    public class RedisProvider : IRedisProvider
    {
        protected readonly Lazy<IConnectionMultiplexer> Lazy;
        
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
            Lazy = new Lazy<IConnectionMultiplexer>(() => ConnectionMultiplexer.Connect(options));
        }

        public IConnectionMultiplexer Connection => Lazy.Value;

        public IDatabase Database => Connection.GetDatabase();
        public ISubscriber Subscriber => Connection.GetSubscriber();

        public void Dispose()
        {
            Connection.Dispose();
        }
    }
}
