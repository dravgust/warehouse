using Microsoft.Extensions.Logging;
using Vayosoft.Core.SharedKernel.Events;
using Vayosoft.Core.Utilities;

namespace Warehouse.Core.UseCases.Positioning.Events
{
    public class UserEventHandler : IEventHandler<UserEventOccurred>
    {
        private readonly ILogger<UserEventHandler> _logger;

        public UserEventHandler(ILogger<UserEventHandler> logger)
        {
            _logger = logger;
        }

        public Task Handle(UserEventOccurred notification, CancellationToken cancellationToken)
        {
            _logger.LogInformation($"Event Notification => Beacon: {notification.Beacon.MacAddress}| more then 30 min. | {notification.Beacon.ReceivedAt}");

            return Task.CompletedTask;
        }
    }
}
