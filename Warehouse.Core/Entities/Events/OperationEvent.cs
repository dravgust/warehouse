using Vayosoft.Core.SharedKernel.Events.External;
using Warehouse.Core.Entities.Enums;

namespace Warehouse.Core.Entities.Events
{
    public record OperationEvent(string SourceId, OperationType Type, string Name, DateTimeOffset TimeStamp, string ProviderName) : IExternalEvent
    {
        public static OperationEvent Create(string sourceId, OperationType type, DateTime created, string providerName)
        {
            if (string.IsNullOrWhiteSpace(sourceId))
                throw new ArgumentException($"{nameof(sourceId)} can't be empty.");

            if (type == default)
                throw new ArgumentException($"{nameof(type)} needs to be defined.");

            if (created == default)
                throw new ArgumentException($"{nameof(created)} needs to be defined.");

            if (string.IsNullOrWhiteSpace(providerName))
                throw new ArgumentException($"{nameof(providerName)} can't be empty.");

            return new OperationEvent(sourceId, type, $"{type}", created, providerName);
        }
    }
}
