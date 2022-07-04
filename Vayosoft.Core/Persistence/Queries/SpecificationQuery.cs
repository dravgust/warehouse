using Vayosoft.Core.Queries;

namespace Vayosoft.Core.Persistence.Queries
{
    public record SpecificationQuery<TSpecification, TResult>(
        TSpecification Specification
    ) : IQuery<TResult>;
}
