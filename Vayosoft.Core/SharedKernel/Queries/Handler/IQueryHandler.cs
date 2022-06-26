using MediatR;
using Vayosoft.Core.SharedKernel.Queries.Query;

namespace Vayosoft.Core.SharedKernel.Queries.Handler
{
    public interface IQueryHandler<in TQuery, TResponse>: IRequestHandler<TQuery, TResponse>
           where TQuery : IQuery<TResponse>
    {
    }
}
