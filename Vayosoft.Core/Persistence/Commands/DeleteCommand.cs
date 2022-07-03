using Vayosoft.Core.Commands;

namespace Vayosoft.Core.Persistence.Commands
{
    public class DeleteCommand<TKey> : ICommand
    {
        public TKey Id { get; set; }
    }
}
