using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.Core.Application.UseCases.BeaconTracking.Events;
using Warehouse.Core.Application.UseCases.BeaconTracking.Models;
using Warehouse.Core.Application.UseCases.BeaconTracking.Queries;
using Warehouse.Core.Domain.Entities;
using Warehouse.Core.Domain.Events;

namespace Warehouse.Core.Application.UseCases.BeaconTracking
{
    public static class Configuration
    {
        public static IServiceCollection AddAppTrackingServices(this IServiceCollection services) =>
            services
                .AddQueryHandlers()
                .AddEventHandlers();

        private static IServiceCollection AddQueryHandlers(this IServiceCollection services) =>
            services
                .AddQueryHandler<GetTrackedItems, IPagedEnumerable<TrackedItemViewModel>, HandleDashboardByBeacon>()
                .AddQueryHandler<GetBeaconEvents, IPagedEnumerable<BeaconEventDto>, HandleGetBeaconEvents>()
                .AddQueryHandler<GetTrackedItemsByProduct, IEnumerable<TrackedItemByProductDto>, HandleGetDashboardByProduct>()
                .AddQueryHandler<GetTrackedItemsBySite, IEnumerable<TrackedItemBySiteDto>, HandleGetDashboardBySite>()
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