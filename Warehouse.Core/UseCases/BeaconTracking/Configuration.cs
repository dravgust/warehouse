using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.Core.Domain.Entities;
using Warehouse.Core.Domain.Events;
using Warehouse.Core.UseCases.BeaconTracking.Events;
using Warehouse.Core.UseCases.BeaconTracking.Models;
using Warehouse.Core.UseCases.BeaconTracking.Queries;

namespace Warehouse.Core.UseCases.BeaconTracking
{
    public static class Configuration
    {
        public static IServiceCollection AddAppTrackingServices(this IServiceCollection services) =>
            services
                .AddQueryHandlers()
                .AddEventHandlers();

        private static IServiceCollection AddQueryHandlers(this IServiceCollection services) =>
            services
                .AddQueryHandler<GetDashboardByBeacon, IPagedEnumerable<DashboardByBeacon>, HandleDashboardByBeacon>()
                .AddQueryHandler<GetBeaconEvents, IPagedEnumerable<BeaconEventDto>, HandleGetBeaconEvents>()
                .AddQueryHandler<GetDashboardByProduct, IEnumerable<DashboardByProduct>, HandleGetDashboardByProduct>()
                .AddQueryHandler<GetDashboardBySite, IEnumerable<DashboardBySite>, HandleGetDashboardBySite>()
                .AddQueryHandler<GetDashboardSite, DashboardBySite, HandleGetIpsStatus>()
                .AddQueryHandler<GetBeaconCharts, BeaconCharts, HandleGetBeaconCharts>()
                .AddQueryHandler<GetBeaconPosition, ICollection<BeaconPosition>, HandleGetBeaconPosition>()
                .AddQueryHandler<GetBeaconTelemetry, BeaconTelemetryDto, HandleGetBeaconTelemetry>()

                .AddQueryHandler<GetUserNotifications, IPagedEnumerable<NotificationEntity>, HandleGetNotifications>()
                .AddStreamQueryHandler<GetUserNotificationStream, NotificationEntity, NotificationStreamQueryHandler>();

        private static IServiceCollection AddEventHandlers(this IServiceCollection services) =>
                services
                    .AddScoped<INotificationHandler<TrackedItemRegistered>, TrackedItemEventHandler>()
                    .AddScoped<INotificationHandler<TrackedItemMoved>, TrackedItemEventHandler>()
                    .AddScoped<INotificationHandler<TrackedItemGotOut>, TrackedItemEventHandler>()
                    .AddScoped<INotificationHandler<TrackedItemEntered>, TrackedItemEventHandler>();
    }
}