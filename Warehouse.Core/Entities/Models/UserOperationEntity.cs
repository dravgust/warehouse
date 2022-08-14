﻿using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Data.MongoDB;
using Warehouse.Core.Entities.Enums;

namespace Warehouse.Core.Entities.Models
{
    [CollectionName("operation_history")]
    public class UserOperationEntity : EntityBase<string>
    {
        public string SourceId { get; set; }

        public string SessionId { get; set; }

        public long UserId { get; set; }
        public string UserName { get; set; }
        public OperationMemberType UserType { get; set; }

        public long ProviderId { get; set; }

        public int Type { get; set; }
        public string Name { get; set; }
        public string Info { get; set; }
        public string Error { get; set; }

        public DateTimeOffset Timestamp { get; set; }

        public OperationStatus Status { get; set; }

    }
}