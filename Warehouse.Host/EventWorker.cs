using Vayosoft.Core.Caching;
using Vayosoft.Core.SharedKernel.Events;
using Vayosoft.Core.SharedKernel.ValueObjects;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Persistence;
using Warehouse.Core.UseCases.Tracking.Events;

namespace Warehouse.Host
{
    public class EventWorker : BackgroundService
    {
        private const int CheckingInterval = 60;
        public static readonly TimeSpan Interval = TimeSpan.FromSeconds(CheckingInterval);

        private readonly IDistributedMemoryCache _cache;
        private readonly ILogger<Worker> _logger;

        private readonly IServiceProvider _serviceProvider;

        public EventWorker(
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
                var store = scope.ServiceProvider.GetRequiredService<WarehouseStore>();

                try
                {
                    var range = new DateRange(DateTime.UtcNow.AddMinutes(-60), DateTime.UtcNow.AddMinutes(-30));

                    var result = await store.ListAsync<BeaconReceivedEntity>(
                        b => b.ReceivedAt >  range.Start && b.ReceivedAt < range.End, token);

                    var eventBus = scope.ServiceProvider.GetRequiredService<IEventBus>();
                    foreach (var beacon in result)
                    {
                        await eventBus.Publish(new UserEventOccurred(beacon));
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
}