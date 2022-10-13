using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Aggregates;

namespace Warehouse.Core.Application.Common.Persistence
{
    public interface IAggregateRepository<T> : IRepository<T> where T : class, IAggregate<string>
    { }
}
