using Vayosoft.Core.SharedKernel.Aggregates;
using Vayosoft.Core.Utilities;
using Vayosoft.Data.MongoDB;
using Warehouse.Core.Entities.Enums;
using Warehouse.Core.Entities.Events;
using Warehouse.Core.Entities.ValueObjects;
using Provider = Warehouse.Core.UseCases.Providers.Models.Provider;

namespace Warehouse.Core.Entities.Models
{
    [CollectionName("operation_history")]
    public class OperationHistoryEntity : Aggregate
    {
        public string SourceId { get; private set; } = default!;

        public OperationType Type { get; private set; }

        public string Name { get; private set; } = default!;

        public DateTime Created { get; private set; }

        public DateRange? Occurs { get; private set; }

        public OperationHistoryEntity() { }

        public static OperationHistoryEntity New(Guid id, string sourceId, OperationType type, DateTime created, Provider provider)
        {
            return new OperationHistoryEntity(id == Guid.Empty ? GuidGenerator.New() : id, sourceId, type, created, provider);
        }

        public OperationHistoryEntity(Guid id, string carId, OperationType type, DateTime created, Provider provider)
        {
            if (id == Guid.Empty)
                throw new ArgumentException($"{nameof(id)} cannot be empty.");

            if (string.IsNullOrWhiteSpace(carId))
                throw new ArgumentException($"{nameof(carId)} cannot be empty.");

            if (type == default)
                throw new ArgumentException($"{nameof(type)} cannot be empty.");

            var @event = OperationOccurred.Create(id, carId, type, created, provider.Name);

            Enqueue(@event);
            Apply(@event);
        }

        public void Apply(OperationOccurred @event)
        {
            Id = @event.EventId;
            SourceId = @event.SourceId;
            Type = @event.Type;
            Name = @event.Name;
            Created = @event.Created;
        }
    }
}
