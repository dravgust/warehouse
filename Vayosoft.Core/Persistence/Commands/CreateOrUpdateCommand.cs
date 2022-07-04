using Vayosoft.Core.Commands;
using Vayosoft.Core.SharedKernel.Entities;

namespace Vayosoft.Core.Persistence.Commands
{
    public record CreateOrUpdateCommand<TEntity>(TEntity Entity) : ICommand where TEntity : IEntity;
}
