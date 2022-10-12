﻿using Microsoft.Extensions.Configuration;

namespace Vayosoft.Streaming.Redis.Consumers
{
    public class RedisStreamConsumerConfig
    {
        public string GroupId { set; get; }
        public string ConsumerId { set; get; }
        public string[] BootstrapServers { set; get; }
        public int Interval { set; get; } = 500;
    }

    public class ExternalEventConsumerConfig
    {
        public string[] Topics { get; set; }
    }

    public static class RedisConsumerConfigExtensions
    {
        public static ExternalEventConsumerConfig GetExternalEventConfig(this IConfiguration configuration)
        {
            return configuration.GetSection("ExternalEventConsumer").Get<ExternalEventConsumerConfig>();
        }

        public static RedisStreamConsumerConfig GetRedisConsumerConfig(this IConfiguration configuration)
        {
            return configuration.GetSection("RedisConsumer").Get<RedisStreamConsumerConfig>();
        }
    }
}
