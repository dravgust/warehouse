using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Vayosoft.Redis
{
    public static class Configuration
    {
        public static IServiceCollection AddRedisConnection(this IServiceCollection services)
        {
            services.AddSingleton<IRedisProvider, RedisProvider>();
            services.AddSingleton<IRedisConnectionProvider>(s => s.GetRequiredService<IRedisProvider>());
            services.AddSingleton<IRedisDatabaseProvider>(s => s.GetRequiredService<IRedisProvider>());
            services.AddSingleton<IRedisSubscriberProvider>(s => s.GetRequiredService<IRedisProvider>());
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
