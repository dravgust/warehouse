﻿using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Entities;

namespace Warehouse.Core.Domain.Entities
{
    [Metadata("dolav_notifications")]
    public class NotificationEntity : EntityBase<string>
    {
        public DateTime TimeStamp { get; set; }
        public string AlertId { get; set; }
        public string MacAddress { get; set; }
        public string SourceId { get; set; }
        public long ProviderId { get; set; }
        public DateTime ReceivedAt { get; set; }
    }
}
