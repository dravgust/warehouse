using MediatR;

namespace Vayosoft.Core.Queries.Query
{
    public interface IQuery<out TResponse> : IRequest<TResponse> { }
}
