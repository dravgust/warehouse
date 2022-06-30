
namespace Vayosoft.Core.SharedKernel.Queries.Query
{
    public record SpecificationQuery<TSpecification, TResult>(
        TSpecification Specification
    ) : IQuery<TResult>;
}
