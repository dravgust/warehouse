using Vayosoft.Core.Queries;

namespace Vayosoft.Core.Persistence.Queries.Query
{
    public record SingleQuery<TResult>(
        object Id
    ) : IQuery<TResult>;
}
