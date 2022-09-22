using Microsoft.Extensions.DependencyInjection;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.Core.UseCases.Management.Models;
using Warehouse.Core.UseCases.Management.Queries;
using Vayosoft.Core.Queries;
using Vayosoft.Core.Commands;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.Management.Commands;
using ErrorOr;

namespace Warehouse.Core.UseCases.Management
{ 
    public static class Configuration
    {
        public static IServiceCollection AddAppManagementServices(this IServiceCollection services) =>
            services
                .AddQueryHandlers()
                .AddCommandHandlers();

        private static IServiceCollection AddQueryHandlers(this IServiceCollection services) =>
            services
                .AddQueryHandler<GetProductMetadata, Metadata, HandleGetProductMetadata>()
                .AddQueryHandler<GetProductItemMetadata, Metadata, HandlerGetProductItemMetadata>()
                .AddQueryHandler<GetRegisteredBeaconList, IEnumerable<string>, HandleGetRegisteredBeaconList>()
                .AddQueryHandler<GetRegisteredGwList, IEnumerable<string>, RegisteredGwQueryHandler>()
                .AddQueryHandler<GetTrackedItems, IPagedEnumerable<TrackedItemDto>, HandleGetProductItems>()
                .AddQueryHandler<GetProducts, IPagedEnumerable<ProductEntity>, HandleGetProducts>()
                .AddQueryHandler<GetSites, IPagedEnumerable<WarehouseSiteEntity>, HandleGetSites>()
                .AddQueryHandler<GetAlerts, IPagedEnumerable<AlertEntity>, HandleGetAlerts>()
                .AddQueryHandler<GetTrackedItems, IPagedEnumerable<TrackedItemDto>, HandleGetProductItems>()
                ;

        
        private static IServiceCollection AddCommandHandlers(this IServiceCollection services) =>
            services
                .AddCommandHandler<SetWarehouseSite, HandleSetWarehouseSite>()
                .AddCommandHandler<DeleteWarehouseSite, HandleDeleteWarehouseSite>()
                .AddCommandHandler<SetGatewayToSite, HandleSetGatewayToSite>()
                .AddCommandHandler<RemoveGatewayFromSite, HandleRemoveGatewayFromSite>()
                .AddCommandHandler<SetTrackedItem, HandleSetBeacon>()
                .AddCommandHandler<SetAlert, HandleSetAlert>()
                .AddCommandHandler<DeleteAlert, HandleDeleteAlert>()
                .AddCommandHandler<DeleteTrackedItem, HandleDeleteTrackedItem>()
                .AddCommandHandler<SetProduct, HandleSetProduct>()
                .AddCommandHandler<DeleteProduct, HandleDeleteProduct>()
        ;
    }
}