using LanguageExt.Common;
using Microsoft.Extensions.DependencyInjection;
using Vayosoft.Commands;
using Vayosoft.Queries;
using Vayosoft.Commons.Models.Pagination;
using Warehouse.Core.Application.SiteManagement.Commands;
using Warehouse.Core.Application.SiteManagement.Models;
using Warehouse.Core.Application.SiteManagement.Queries;
using Warehouse.Core.Domain.Entities;
using Warehouse.Core.Domain.ValueObjects;

namespace Warehouse.Core.Application.SiteManagement
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
                .AddCommandHandler<SetTrackedItem, Result<TrackedItem>, HandleSetBeacon>()
                .AddCommandHandler<SetAlert, HandleSetAlert>()
                .AddCommandHandler<DeleteAlert, HandleDeleteAlert>()
                .AddCommandHandler<DeleteTrackedItem, HandleDeleteTrackedItem>()
                .AddCommandHandler<SetProduct, HandleSetProduct>()
                .AddCommandHandler<DeleteProduct, HandleDeleteProduct>()
        ;
    }
}