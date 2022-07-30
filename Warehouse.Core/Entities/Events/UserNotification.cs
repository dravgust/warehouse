using Vayosoft.Core.SharedKernel.Events;
using Vayosoft.Core.SharedKernel.ValueObjects;

namespace Warehouse.Core.Entities.Events
{
    public record UserNotification(string UserId, MacAddress SourceId, string Message, string ProviderName) : IEvent
    {
        public static UserNotification Create(string userId, MacAddress sourceId, string message, string providerName)
        {
            if (string.IsNullOrWhiteSpace(userId))
                throw new ArgumentException($"{nameof(userId)} can't be empty.");

            if (string.IsNullOrWhiteSpace(sourceId))
                throw new ArgumentException($"{nameof(sourceId)} can't be empty.");

            if (string.IsNullOrWhiteSpace(providerName))
                throw new ArgumentException($"{nameof(providerName)} can't be empty.");

            return new UserNotification(userId, sourceId, message, providerName);
        }
    }
}
