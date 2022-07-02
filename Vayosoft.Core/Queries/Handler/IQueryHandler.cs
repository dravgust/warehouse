using MediatR;
using Vayosoft.Core.Queries.Query;

namespace Vayosoft.Core.Queries.Handler
{
    public interface IQueryHandler<in TQuery, TResponse>: IRequestHandler<TQuery, TResponse>
           where TQuery : IQuery<TResponse>
    {
    }
}
