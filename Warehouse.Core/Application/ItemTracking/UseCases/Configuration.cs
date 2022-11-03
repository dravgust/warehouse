using Microsoft.Extensions.DependencyInjection;
using Vayosoft.Queries;
using Warehouse.Core.Application.ItemTracking.UseCases.Queries;
using Warehouse.PositioningSystem.Entities;

namespace Warehouse.Core.Application.ItemTracking.UseCases
{
    public static class Configuration
    {
        public static IServiceCollection AddPositioningSystemServices(this IServiceCollection services) =>
            services
                .AddQueryHandlers()
                .AddCommandHandlers();

        private static IServiceCollection AddQueryHandlers(this IServiceCollection services) =>
            services
                .AddQueryHandler<GetGenericSite, GenericSite, HandleGetGenericSite>()
                ;


        private static IServiceCollection AddCommandHandlers(this IServiceCollection services) =>
            services
        ;
    }
}
