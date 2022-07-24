using Microsoft.Extensions.Configuration;

namespace Vayosoft.Streaming.Redis.Producers
{
    public class RedisProducerConfig
    { 
        public string? Topic { get; set; }
    }

    public static class RedisProducerConfigExtensions
    {
        public static RedisProducerConfig GetRedisProducerConfig(this IConfiguration configuration)
        {
            return configuration.GetSection(nameof(RedisProducer)).Get<RedisProducerConfig>();
        }
    }
}
