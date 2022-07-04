using Vayosoft.Core.Queries;

namespace Vayosoft.Core.Persistence.Queries
{
    public record SingleQuery<TResult>(
        object Id
    ) : IQuery<TResult>;
}
