using Vayosoft.Core.SharedKernel.Events.External;
using Warehouse.Core.Entities.Enums;

namespace Warehouse.Core.Entities.Events
{
    public record OperationOccurred(Guid EventId, string SourceId, OperationType Type, string Name, DateTime Created, string ProviderName) : IExternalEvent
    {
        public static OperationOccurred Create(Guid eventId, string sourceId, OperationType type, DateTime created, string providerName)
        {
            if (eventId == default)
                throw new ArgumentException($"{nameof(eventId)} needs to be defined.");

           
            if (string.IsNullOrWhiteSpace(sourceId))
                throw new ArgumentException($"{nameof(sourceId)} can't be empty.");

          
            if (type == default)
                throw new ArgumentException($"{nameof(type)} needs to be defined.");

            if (created == default)
                throw new ArgumentException($"{nameof(created)} needs to be defined.");

            if (string.IsNullOrWhiteSpace(providerName))
                throw new ArgumentException($"{nameof(providerName)} can't be empty.");

            return new OperationOccurred(eventId, sourceId, type, $"{type}", created, providerName);
        }
    }
}
