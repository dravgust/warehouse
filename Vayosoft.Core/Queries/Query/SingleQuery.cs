namespace Vayosoft.Core.Queries.Query
{
    public record SingleQuery<TResult>(
        object Id
    ) : IQuery<TResult>;
}
