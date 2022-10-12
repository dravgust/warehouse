﻿using Microsoft.Extensions.DependencyInjection;
using Vayosoft.Core.Queries;
using Warehouse.Core.Application.PositioningSystem.Domain;
using Warehouse.Core.Application.PositioningSystem.UseCases;

namespace Warehouse.Core.Application.PositioningSystem
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
