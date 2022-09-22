using MediatR;
using Vayosoft.Caching;
using Vayosoft.Core;
using Vayosoft.Streaming.Redis;
using Warehouse.Core.Entities.Events;
using Warehouse.Core.Services;
using Warehouse.Infrastructure;

namespace Warehouse.Host
{
    public static class Configuration
    {
        public static IServiceCollection AddApplication(this IServiceCollection services, IConfiguration configuration)
        {
            services
                .AddCaching(configuration)
                .AddRedisProducerAndConsumer()
                .AddCoreServices();

            services.AddEventHandlers();

            services.AddScoped<IUserContext, ServiceContext>();

            services.AddInfrastructure(configuration);

            services.AddHostedService<Worker>();
            services.AddHostedService<NotificationWorker>();
            //services.AddHostedService<HostedService>();

            return services;
        }

        private static IServiceCollection AddEventHandlers(this IServiceCollection services) =>
            services
                .AddScoped<INotificationHandler<TrackedItemRegistered>, TrackedItemEventHandler>()
                .AddScoped<INotificationHandler<TrackedItemMoved>, TrackedItemEventHandler>()
                .AddScoped<INotificationHandler<TrackedItemGotOut>, TrackedItemEventHandler>()
                .AddScoped<INotificationHandler<TrackedItemEntered>, TrackedItemEventHandler>();
    }
}
