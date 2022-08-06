using Vayosoft.Core.Caching;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.SharedKernel.Events;
using Vayosoft.Core.SharedKernel.ValueObjects;
using Vayosoft.Core.Utilities;
using Vayosoft.Data.MongoDB;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Persistence;
using Warehouse.Core.UseCases.BeaconTracking.Events;

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
            while (!token.IsCancellationRequested)
            {
                _logger.LogInformation("Event worker running at: {time}", DateTimeOffset.Now);

                using var scope = _serviceProvider.CreateScope(); 
                var alertRepository = scope.ServiceProvider.GetRequiredService<IReadOnlyRepository<AlertEntity>>();
                var beaconReceivedRepository = scope.ServiceProvider.GetRequiredService<IReadOnlyRepository<BeaconReceivedEntity>>();
                var notifyReadRepository = scope.ServiceProvider.GetRequiredService<IReadOnlyRepository<NotificationEntity>>();
                var notificationRepository = scope.ServiceProvider.GetRequiredService<IRepository<NotificationEntity>>();
                
                try
                {
                    //var registeredBeacons = await store.ListAsync<BeaconRegisteredEntity>(cancellationToken: token);
                    var alerts = await alertRepository.ListAsync(cancellationToken: token);

                    foreach (var alert in alerts.Where(a => a.Enabled))
                    {
                        var result = await beaconReceivedRepository.ListAsync(b => 
                            b.ReceivedAt < DateTime.UtcNow.AddSeconds(-alert.CheckPeriod), token);

                        if (result.Any())
                        {
                            //var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();
                            foreach (var beacon in result)
                            {
                                //await eventBus.Publish(UserNotification.Create);
                                var notified = await notifyReadRepository.SingleOrDefaultAsync(n => 
                                    n.AlertId == alert.Id && n.MacAddress == beacon.MacAddress, token);
                                if(notified != null) continue;

                                var notification = new NotificationEntity
                                {
                                    TimeStamp = DateTime.UtcNow,
                                    AlertId = alert.Id,
                                    MacAddress = beacon.Id,
                                    SourceId = beacon.SourceId,
                                    ReceivedAt = beacon.ReceivedAt
                                };
                                await notificationRepository.AddAsync(notification, token);
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