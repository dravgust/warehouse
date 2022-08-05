using Vayosoft.Caching;
using Vayosoft.Core;
using Vayosoft.Streaming.Redis;
using Warehouse.Core;
using Warehouse.Core.UseCases.Administration.ValueObjects;

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

            services.AddScoped<IdentityContext>();

            services.AddWarehouseDependencies(configuration);

            services.AddHostedService<Worker>();
            services.AddHostedService<NotificationWorker>();
            //services.AddHostedService<HostedService>();

            return services;
        }
    }
}
