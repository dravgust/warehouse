namespace Vayosoft.Core.SharedKernel.Queries.Query
{
    public record GetEntityByIdQuery<TResult>(
        object Id
    ) : IQuery<TResult>;
}
