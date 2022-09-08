using System.Threading;
using System.Threading.Tasks;
using MediatR;

namespace Vayosoft.Core.Queries
{
    public class QueryBus: IQueryBus
    {
        private readonly IMediator mediator;

        public QueryBus(IMediator mediator)
        {
            this.mediator = mediator;
        }

        public Task<TResponse> Send<TResponse>(IQuery<TResponse> query, CancellationToken cancellationToken = default)
        {
            return mediator.Send(query, cancellationToken);
        }
    }
}
