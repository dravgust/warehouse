using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Vayosoft.Core.Persistence.Queries;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Data.MongoDB.QueryHandlers;
using Warehouse.Core.Entities.Events;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.BeaconTracking.Events;
using Warehouse.Core.UseCases.BeaconTracking.Models;
using Warehouse.Core.UseCases.BeaconTracking.Queries;
using Warehouse.Core.UseCases.BeaconTracking.Specifications;

namespace Warehouse.Core.UseCases.BeaconTracking
{
    internal static class Configuration
    {
        public static IServiceCollection AddWarehouseTrackingServices(this IServiceCollection services) =>
            services
                .AddQueryHandlers()
                .AddEventHandlers();

        private static IServiceCollection AddQueryHandlers(this IServiceCollection services) =>
            services
                .AddQueryHandler<GetDashboardByBeacon, IPagedEnumerable<DashboardByBeacon>, HandleDashboardByBeacon>()
                .AddQueryHandler<GetBeaconEvents, IPagedEnumerable<BeaconEventDto>, HandleGetBeaconEvents>()
                //.AddQueryHandler<SpecificationQuery<NotificationSpec, IPagedEnumerable<NotificationEntity>>, IPagedEnumerable<NotificationEntity>,
                //    MongoPagingQueryHandler<NotificationSpec, NotificationEntity>>()
                .AddQueryHandler<GetUserNotifications, IPagedEnumerable<NotificationEntity>, HandleGetNotifications>()
                .AddQueryHandler<GetDashboardByProduct, IEnumerable<DashboardByProduct>, HandleGetDashboardByProduct>()
                .AddQueryHandler<GetDashboardBySite, IEnumerable<DashboardBySite>, HandleGetDashboardBySite>()
                .AddQueryHandler<GetDashboardSite, DashboardBySite, HandleGetIpsStatus>()
                .AddQueryHandler<GetBeaconTelemetry2, BeaconTelemetry2Dto, HandleGetBeaconTelemetry>()
                .AddQueryHandler<GetBeaconTelemetry, BeaconTelemetryDto, HandleGetBeaconTelemetry>()
                //.AddQueryHandler<SpecificationQuery<BeaconEventSpec, IPagedEnumerable<BeaconEventEntity>>, IPagedEnumerable<BeaconEventEntity>,
                //    MongoPagingQueryHandler<BeaconEventSpec, BeaconEventEntity>>()
                .AddQueryHandler<SpecificationQuery<BeaconPositionSpec, IPagedEnumerable<BeaconReceivedEntity>>, IPagedEnumerable<BeaconReceivedEntity>,
                    MongoPagingQueryHandler<BeaconPositionSpec, BeaconReceivedEntity>>();

            private static IServiceCollection AddEventHandlers(this IServiceCollection services) =>
                services
                    .AddScoped<INotificationHandler<TrackedItemRegistered>, TrackedItemEventHandler>()
                    .AddScoped<INotificationHandler<TrackedItemMoved>, TrackedItemEventHandler>()
                    .AddScoped<INotificationHandler<TrackedItemGotOut>, TrackedItemEventHandler>()
                    .AddScoped<INotificationHandler<TrackedItemEntered>, TrackedItemEventHandler>();
    }
}