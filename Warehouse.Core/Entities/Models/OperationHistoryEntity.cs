using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Data.MongoDB;
using Warehouse.Core.Entities.Enums;

namespace Warehouse.Core.Entities.Models
{
    [CollectionName("operation_history")]
    public class OperationHistoryEntity : EntityBase<string>
    {
        public string SourceId { get; set; }

        public long ActorId { get; set; }
        public string ActorName { get; set; }
        public string ActorType { get; set; }

        public string SessionId { get; set; }
        public long ProviderId { get; set; }

        public int OpType { get; set; }
        public string OpName { get; set; }
        public string OpInfo { get; set; }

        public DateTimeOffset Start { get; set; }
        public DateTimeOffset End { get; set; }
        public OperationStatus OpStatus { get; set; }

    }
}
