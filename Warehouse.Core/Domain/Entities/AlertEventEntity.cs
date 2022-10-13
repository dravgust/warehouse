using Vayosoft.Core.Mapping;
using Vayosoft.Core.SharedKernel.Aggregates;
using Vayosoft.Core.SharedKernel.Entities;

namespace Warehouse.Core.Domain.Entities
{
    [AggregateName("dolav_notifications")]
    public class AlertEventEntity : EntityBase<string>, IAggregateRoot
    {
        public DateTime TimeStamp { get; set; }
        public string AlertId { get; set; }
        public string MacAddress { get; set; }
        public string SourceId { get; set; }
        public long ProviderId { get; set; }
        public DateTime ReceivedAt { get; set; }
    }
}
