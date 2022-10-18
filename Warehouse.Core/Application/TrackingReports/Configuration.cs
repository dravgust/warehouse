using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Warehouse.Core.Application.TrackingReports.Events;
using Warehouse.Core.Application.TrackingReports.Models;
using Warehouse.Core.Application.TrackingReports.Queries;
using Warehouse.Core.Domain.Entities;
using Warehouse.Core.Domain.Events;
using Warehouse.Core.Domain.ValueObjects;

namespace Warehouse.Core.Application.TrackingReports
{
    public static class Configuration
    {
        public static IServiceCollection AddAppTrackingServices(this IServiceCollection services) =>
            services
                .AddQueryHandlers()
                .AddEventHandlers();

        private static IServiceCollection AddQueryHandlers(this IServiceCollection services) =>
            services
                .AddQueryHandler<GetTrackedItems, IPagedEnumerable<TrackedItemData>, HandleDashboardByBeacon>()
                .AddQueryHandler<GetEventNotifications, IPagedEnumerable<EventNotification>, HandleGetEventNotifications>()
                .AddQueryHandler<GetTrackedItemsByProduct, IEnumerable<TrackedItemByProductDto>, HandleGetDashboardByProduct>()
                .AddQueryHandler<GetTrackedItemsBySite, IEnumerable<TrackedItemBySiteDto>, HandleGetDashboardBySite>()
                .AddQueryHandler<GetBeaconTelemetryReport, BeaconTelemetryReport, HandleGetBeaconTelemetryReport>()
                .AddQueryHandler<GetBeaconPosition, ICollection<BeaconPosition>, HandleGetBeaconPosition>()
                .AddQueryHandler<GetBeaconTelemetry, BeaconTelemetryDto, HandleGetBeaconTelemetry>()

                .AddQueryHandler<GetUserNotifications, IPagedEnumerable<UserNotification>, HandleGetUserNotifications>()
                .AddStreamQueryHandler<GetUserNotificationStream, AlertEvent, NotificationStreamQueryHandler>();

        private static IServiceCollection AddEventHandlers(this IServiceCollection services) =>
                services
                    .AddScoped<INotificationHandler<TrackedItemRegistered>, TrackedItemEventHandler>()
                    .AddScoped<INotificationHandler<TrackedItemMoved>, TrackedItemEventHandler>()
                    .AddScoped<INotificationHandler<TrackedItemGotOut>, TrackedItemEventHandler>()
                    .AddScoped<INotificationHandler<TrackedItemEntered>, TrackedItemEventHandler>();
    }
}