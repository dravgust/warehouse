using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.Core.UseCases.Management.Models;
using Warehouse.Core.UseCases.Management.Queries;
using Vayosoft.Core.Queries;
using Vayosoft.Core.Commands;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.Management.Commands;

namespace Warehouse.Core.UseCases.Management
{ 
    internal static class Configuration
    {
        public static IServiceCollection AddWarehouseManagementServices(this IServiceCollection services) =>
            services
                .AddQueryHandlers()
                .AddCommandHandlers();

        private static IServiceCollection AddQueryHandlers(this IServiceCollection services) =>
            services
                .AddQueryHandler<GetProductMetadata, ProductMetadata, ProductQueryHandler>()
                .AddQueryHandler<GetProductItemMetadata, ProductMetadata, ProductQueryHandler>()
                .AddQueryHandler<GetRegisteredBeaconList, IEnumerable<string>, WarehouseQueryHandler>()
                .AddQueryHandler<GetRegisteredGwList, IEnumerable<string>, GetRegisteredGwList.RegisteredGwQueryHandler>()
                .AddQueryHandler<GetProductItems, IPagedEnumerable<ProductItemDto>, WarehouseQueryHandler>();

        private static IServiceCollection AddCommandHandlers(this IServiceCollection services) =>
            services
                .AddCommandHandler<SetWarehouseSite, WarehouseCommandHandler>()
                .AddCommandHandler<DeleteWarehouseSite, WarehouseCommandHandler>()
                .AddCommandHandler<SetGatewayToSite, WarehouseCommandHandler>()
                .AddCommandHandler<RemoveGatewayFromSite, WarehouseCommandHandler>()
                .AddCommandHandler<SetBeacon, WarehouseCommandHandler>()
                .AddCommandHandler<SetProduct, ProductCommandHandler>()
                .AddCommandHandler<DeleteProduct, ProductCommandHandler>();
    }
}