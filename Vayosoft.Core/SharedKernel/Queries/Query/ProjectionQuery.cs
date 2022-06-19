namespace Vayosoft.Core.SharedKernel.Queries.Query
{
    public record ProjectionQuery<TSpecification, TResult>(
        TSpecification Specification
    ) : IQuery<TResult>;
}
