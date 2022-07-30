using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Vayosoft.Core.SharedKernel.Aggregates;
using Vayosoft.Core.SharedKernel.Events;

namespace Vayosoft.Core.Persistence
{
    public interface IEventStore
    {
        Task SaveAsync(IAggregate aggregate, CancellationToken cancellationToken = default);
        Task<IEnumerable<IEvent>> Get(
            Guid aggregateId,
            int fromVersion,
            CancellationToken cancellationToken = default);
    }
}
