using Vayosoft.Caching;
using Vayosoft.Core;
using Vayosoft.Streaming.Redis;
using Warehouse.Core.Services;
using Warehouse.Infrastructure;

namespace Warehouse.Host
{
    public static class Configuration
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddCoreServices()
                .AddRedisProducerAndConsumer()
                .AddCaching(configuration);

            services.AddScoped<IUserContext, ServiceContext>();

            services.AddWarehouseDependencies(configuration);

            services.AddHostedService<Worker>();
            services.AddHostedService<NotificationWorker>();
            //services.AddHostedService<HostedService>();

            return services;
        }
    }
}
