using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Vayosoft.Core.Persistence.Queries;
using Vayosoft.Core.Queries;
using Vayosoft.Core.SharedKernel.Models.Pagination;
using Vayosoft.Data.MongoDB.QueryHandlers;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.Tracking.Events;
using Warehouse.Core.UseCases.Tracking.Models;
using Warehouse.Core.UseCases.Tracking.Queries;
using Warehouse.Core.UseCases.Tracking.Specifications;

namespace Warehouse.Core.UseCases.Tracking
{
    internal static class Configuration
    {
        public static IServiceCollection AddPositionReportServices(this IServiceCollection services) =>
            services
                .AddQueryHandlers()
                .AddEventHandlers();

        private static IServiceCollection AddQueryHandlers(this IServiceCollection services) =>
            services
                .AddQueryHandler<GetAssets, IPagedEnumerable<AssetDto>, AssetsQueryHandler>()
                .AddQueryHandler<GetBeaconEvents, IPagedEnumerable<BeaconEventDto>, AssetsQueryHandler>()
                .AddQueryHandler<GetAssetInfo, IEnumerable<AssetInfo>, AssetsQueryHandler>()
                .AddQueryHandler<GetSiteInfo, IEnumerable<IndoorPositionStatusDto>, AssetsQueryHandler>()
                .AddQueryHandler<GetIpsStatus, IndoorPositionStatusDto, AssetsQueryHandler>()
                .AddQueryHandler<GetBeaconTelemetry2, BeaconTelemetry2Dto, AssetsQueryHandler>()
                .AddQueryHandler<GetBeaconTelemetry, BeaconTelemetryDto, AssetsQueryHandler>()
                .AddQueryHandler<SpecificationQuery<BeaconEventSpec, IPagedEnumerable<BeaconEventEntity>>, IPagedEnumerable<BeaconEventEntity>,
                    MongoPagingQueryHandler<BeaconEventSpec, BeaconEventEntity>>()
                .AddQueryHandler<SpecificationQuery<BeaconPositionSpec, IPagedEnumerable<BeaconReceivedEntity>>, IPagedEnumerable<BeaconReceivedEntity>,
                    MongoPagingQueryHandler<BeaconPositionSpec, BeaconReceivedEntity>>();

            private static IServiceCollection AddEventHandlers(this IServiceCollection services) =>
                services
                    .AddScoped<INotificationHandler<UserEventOccurred>, UserEventHandler>();
    }
}