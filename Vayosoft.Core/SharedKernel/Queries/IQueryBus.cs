using System.Threading;
using System.Threading.Tasks;
using Vayosoft.Core.SharedKernel.Queries.Query;

namespace Vayosoft.Core.SharedKernel.Queries
{
    public interface IQueryBus
    {
        Task<TResponse> Send<TQuery, TResponse>(TQuery query, CancellationToken cancellationToken = default) where TQuery : IQuery<TResponse>;
    }
}
