
namespace Vayosoft.Core.Queries.Query
{
    public record SpecificationQuery<TSpecification, TResult>(
        TSpecification Specification
    ) : IQuery<TResult>;
}
