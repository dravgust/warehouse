using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Vayosoft.Core.SharedKernel.Events.External;
using Vayosoft.Redis;
using Vayosoft.Streaming.Consumers;
using Vayosoft.Streaming.Redis.Consumers;
using Vayosoft.Streaming.Redis.Producers;

namespace Vayosoft.Streaming.Redis
{
    public static class Configuration
    {
        public static IServiceCollection AddRedisProducer(this IServiceCollection services)
        {
            //using TryAdd to support mocking, without that it won't be possible to override in tests
            services.TryAddScoped<IExternalEventProducer, RedisProducer>();
            return services;
        }

        public static IServiceCollection AddRedisConsumer(this IServiceCollection services)
        {
            services.TryAddSingleton<IRedisConsumer<ConsumeResult>, RedisConsumerGroup>();
            //using TryAdd to support mocking, without that it won't be possible to override in tests
            services.TryAddSingleton<IExternalEventConsumer, RedisExternalEventConsumer>();

            return services.AddExternalEventConsumerBackgroundWorker();
        }

        public static IServiceCollection AddRedisProducerAndConsumer(this IServiceCollection services)
        {
            services.AddRedisConnection();
            return services
                .AddRedisProducer()
                .AddRedisConsumer();
        }
    }
}
