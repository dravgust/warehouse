using MediatR;

namespace Vayosoft.Core.SharedKernel.Queries.Query
{
    public interface IQuery<out TResponse> : IRequest<TResponse> { }
}
