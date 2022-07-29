using Vayosoft.Caching;
using Vayosoft.Core;
using Vayosoft.Streaming.Redis;
using Warehouse.Core;

namespace Warehouse.Host
{
    public static class Configuration
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services.AddWarehouseDependencies(configuration);

            services
                .AddCoreServices()
                .AddRedisProducerAndConsumer()
                .AddCaching(configuration);

            services.AddHostedService<Worker>();
            services.AddHostedService<EventWorker>();
            //services.AddHostedService<HostedService>();

            return services;
        }
    }
}
