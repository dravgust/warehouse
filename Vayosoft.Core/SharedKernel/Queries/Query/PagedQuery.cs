
namespace Vayosoft.Core.SharedKernel.Queries.Query
{
    public record PagedQuery<TSpecification, TResult>(
        TSpecification Specification
    ) : IQuery<TResult>;

}
