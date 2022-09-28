using Vayosoft.Core.Caching;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.Specifications;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Persistence;
using Warehouse.Core.UseCases.Administration.Models;

namespace Warehouse.Host
{
    public class NotificationWorker : BackgroundService
    {
        private const int CheckingInterval = 60;
        public static readonly TimeSpan Interval = TimeSpan.FromSeconds(CheckingInterval);

        private readonly IDistributedMemoryCache _cache;
        private readonly ILogger<Worker> _logger;

        private readonly IServiceProvider _serviceProvider;

        public NotificationWorker(
            IServiceProvider serviceProvider,
            IDistributedMemoryCache cache,
            ILogger<Worker> logger)
        {
            _serviceProvider = serviceProvider;
            _cache = cache;
            _logger = logger;
        }

        protected override async Task ExecuteAsync(CancellationToken token)
        {
            var providers = new List<long> { 2, 1000 };

            while (!token.IsCancellationRequested)
            {
                _logger.LogDebug("Event worker running at: {time}", DateTimeOffset.Now);

                using var scope = _serviceProvider.CreateScope(); 
                var alertRepository = scope.ServiceProvider.GetRequiredService<IReadOnlyRepository<AlertEntity>>();
                var notifyReadRepository = scope.ServiceProvider.GetRequiredService<IReadOnlyRepository<NotificationEntity>>();
                var notificationRepository = scope.ServiceProvider.GetRequiredService<IRepositoryBase<NotificationEntity>>();
                var store = scope.ServiceProvider.GetRequiredService<WarehouseStore>();
                
                try
                {
                    foreach (var providerId in providers)
                    {
                        //var registeredBeacons = await store.ListAsync<BeaconRegisteredEntity>(cancellationToken: token);
                        var alertSpec = new Specification<AlertEntity>(s => s.ProviderId == providerId);
                        var alerts = await alertRepository.ListAsync(alertSpec, token);

                        foreach (var alert in alerts.Where(a => a.Enabled))
                        {
                            var beaconSpec = new Specification<TrackedItem>(b =>
                                b.ReceivedAt < DateTime.UtcNow.AddSeconds(-alert.CheckPeriod) &&
                                b.ProviderId == providerId);

                            var result = await store.TrackedItems.ListAsync(beaconSpec, token);

                            if (result.Any())
                            {
                                //var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();
                                foreach (var beacon in result)
                                {
                                    var beaconItem = await store.TrackedItems
                                        .FirstOrDefaultAsync(q => q.Id.Equals(beacon.Id), token);
                                    if (beaconItem == null) continue;

                                    //await eventBus.Publish(UserNotification.Create);
                                    var notified = await notifyReadRepository.FirstOrDefaultAsync(n =>
                                        n.AlertId == alert.Id && n.MacAddress == beacon.Id, token);
                                    if (notified != null) continue;

                                    var notification = new NotificationEntity
                                    {
                                        TimeStamp = DateTime.UtcNow,
                                        AlertId = alert.Id,
                                        MacAddress = beacon.Id,
                                        SourceId = beacon.SourceId,
                                        ProviderId = alert.ProviderId,
                                        ReceivedAt = beacon.ReceivedAt
                                    };
                                    await notificationRepository.AddAsync(notification, token);
                                }
                            }
                        }
                    }

                    await Task.Delay(Interval, token);
                }
                catch (OperationCanceledException)
                {
                }
                catch (Exception e)
                {
                    _logger.LogError($"{e.Message}\r\n{e.StackTrace}");
                }
            }
        }
    }
    
    //public record UserNotification(string UserId, MacAddress SourceId, string Message, string ProviderName) : IEvent
    //{
    //    public static UserNotification Create(string userId, MacAddress sourceId, string message, string providerName)
    //    {
    //        if (string.IsNullOrWhiteSpace(userId))
    //            throw new ArgumentException($"{nameof(userId)} can't be empty.");

    //        if (string.IsNullOrWhiteSpace(sourceId))
    //            throw new ArgumentException($"{nameof(sourceId)} can't be empty.");

    //        if (string.IsNullOrWhiteSpace(providerName))
    //            throw new ArgumentException($"{nameof(providerName)} can't be empty.");

    //        return new UserNotification(userId, sourceId, message, providerName);
    //    }
    //}
}