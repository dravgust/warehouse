using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Vayosoft.Data.Redis
{
    public static class Config
    {
        public static IServiceCollection AddRedisConnection(this IServiceCollection services)
        {

            services.AddSingleton<RedisConnection>();
            services.AddSingleton<IRedisDatabaseConnection>(s => s.GetRequiredService<RedisConnection>());
            services.AddSingleton<IRedisSubscriberConnection>(s => s.GetRequiredService<RedisConnection>());
            return services;
        }

        public static IServiceCollection AddRedisCache(this IServiceCollection services, IConfiguration config)
        {
            services.AddStackExchangeRedisCache(options =>
            {
                options.Configuration = config.GetValue<string>("CacheSettings:ConnectionString");
                options.InstanceName = "CacheInstance";
            });

            return services;
        }
    }
}
