﻿using Vayosoft.Core.Mapping;
using Vayosoft.Core.SharedKernel.Aggregates;
using Vayosoft.Core.SharedKernel.Entities;

namespace Warehouse.Core.Domain.Entities
{
    [AggregateName("dolav_alerts")]
    public class AlertEntity : EntityBase<string>, IAggregateRoot, IProvider<long>
    {
        public string Name { get; set; }
        public int CheckPeriod { get; set; }
        public bool Enabled { get; set; }
        public long ProviderId { get; set; }
        object IProvider.ProviderId => ProviderId;
    }
}
