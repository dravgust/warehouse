using Vayosoft.Core.Commands;
using Vayosoft.Core.SharedKernel.Entities;

namespace Vayosoft.Core.Persistence.Commands
{
    public class CreateOrUpdateCommand : IEntity, ICommand
    {
        public object Id { get; }
    }
}
