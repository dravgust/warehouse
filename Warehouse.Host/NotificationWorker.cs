using Vayosoft.Core.Caching;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Persistence;

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
            var alerts = new List<AlertEntity>
            {
                new AlertEntity
                {
                    Id = GuidGenerator.New().ToString(),
                    CheckPeriod = 5 * 60,
                    Enabled = true
                }
            };

            while (!token.IsCancellationRequested)
            {
                _logger.LogInformation("Event worker running at: {time}", DateTimeOffset.Now);

                using var scope = _serviceProvider.CreateScope(); 
                var store = scope.ServiceProvider.GetRequiredService<WarehouseStore>();
                
                try
                {
                    //var registeredBeacons = await store.ListAsync<BeaconRegisteredEntity>(cancellationToken: token);

                    foreach (var alert in alerts.Where(a => a.Enabled))
                    {
                        var result = await store.ListAsync<BeaconReceivedEntity>(b => 
                            b.ReceivedAt < DateTime.UtcNow.AddSeconds(-alert.CheckPeriod), token);

                        if (result.Any())
                        {
                            //var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();
                            foreach (var beacon in result)
                            {
                                //await eventBus.Publish(UserNotification.Create);
                                var notified = await store.SingleOrDefaultAsync<NotificationEntity>(n => 
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
                                await store.AddAsync(notification, token);
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