using MediatR;

namespace Vayosoft.Core.Queries
{
    public interface IQuery<out TResponse> : IRequest<TResponse> { }
}
