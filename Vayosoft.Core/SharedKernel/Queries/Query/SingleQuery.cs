namespace Vayosoft.Core.SharedKernel.Queries.Query
{
    public record SingleQuery<TResult>(
        object Id
    ) : IQuery<TResult>;
}
