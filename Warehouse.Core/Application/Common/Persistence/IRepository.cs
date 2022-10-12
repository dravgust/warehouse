using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Aggregates;

namespace Warehouse.Core.Application.Common.Persistence
{
    public interface IRepository<T> : IRepositoryBase<T> where T : class, IAggregate<string>
    { }
}
