using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Vayosoft.Data.Redis
{
    public static class Config
    {
        public static IServiceCollection AddRedisConnection(this IServiceCollection services)
        {

            services.AddSingleton<RedisProvider>();
            services.AddSingleton<IRedisConnectionProvider>(s => s.GetRequiredService<RedisProvider>());
            services.AddSingleton<IRedisDatabaseProvider>(s => s.GetRequiredService<RedisProvider>());
            services.AddSingleton<IRedisSubscriberProvider>(s => s.GetRequiredService<RedisProvider>());
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
