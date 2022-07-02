using System.Threading;
using System.Threading.Tasks;
using Vayosoft.Core.Queries.Query;

namespace Vayosoft.Core.Queries
{
    public interface IQueryBus
    {
        Task<TResponse> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default);
    }
}
