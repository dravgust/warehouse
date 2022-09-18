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
                .AddQueryHandler<GetProductMetadata, ProductMetadata, HandleGetProductMetadata>()
                .AddQueryHandler<GetProductItemMetadata, ProductMetadata, HandlerGetProductItemMetadata>()
                .AddQueryHandler<GetRegisteredBeaconList, IEnumerable<string>, HandleGetRegisteredBeaconList>()
                .AddQueryHandler<GetRegisteredGwList, IEnumerable<string>, RegisteredGwQueryHandler>()
                .AddQueryHandler<GetBeacons, IPagedEnumerable<ProductItemDto>, HandleGetProductItems>()
                .AddQueryHandler<GetProducts, IPagedEnumerable<ProductEntity>, HandleGetProducts>()
                .AddQueryHandler<GetSites, IPagedEnumerable<WarehouseSiteEntity>, HandleGetSites>()
                .AddQueryHandler<GetAlerts, IPagedEnumerable<AlertEntity>, HandleGetAlerts>()
                .AddQueryHandler<GetBeacons, IPagedEnumerable<ProductItemDto>, HandleGetProductItems>()
                ;

        
        private static IServiceCollection AddCommandHandlers(this IServiceCollection services) =>
            services
                .AddCommandHandler<SetWarehouseSite, HandleSetWarehouseSite>()
                .AddCommandHandler<DeleteWarehouseSite, HandleDeleteWarehouseSite>()
                .AddCommandHandler<SetGatewayToSite, HandleSetGatewayToSite>()
                .AddCommandHandler<RemoveGatewayFromSite, HandleRemoveGatewayFromSite>()
                .AddCommandHandler<SetBeacon, HandleSetBeacon>()
                .AddCommandHandler<SetAlert, HandleSetAlert>()
                .AddCommandHandler<DeleteAlert, HandleDeleteAlert>()
                .AddCommandHandler<DeleteBeacon, HandleDeleteBeacon>()
                .AddCommandHandler<SetProduct, HandleSetProduct>()
                .AddCommandHandler<DeleteProduct, HandleDeleteProduct>()
                .AddCommandHandler<RegisterTrackedItem, ErrorOr<TrackedItem>, HandleRegisterTrackedItem>()
            ;
    }
}