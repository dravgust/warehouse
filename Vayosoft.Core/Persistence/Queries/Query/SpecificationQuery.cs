using Vayosoft.Core.Queries;

namespace Vayosoft.Core.Persistence.Queries.Query
{
    public record SpecificationQuery<TSpecification, TResult>(
        TSpecification Specification
    ) : IQuery<TResult>;
}
