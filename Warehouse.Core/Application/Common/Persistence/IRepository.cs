using Vayosoft.Persistence;
using Vayosoft.Commons.Aggregates;

namespace Warehouse.Core.Application.Common.Persistence
{
    public interface IAggregateRepository<T> : IRepository<T> where T : class, IAggregate<string>
    { }
}
