using MediatR;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Persistence;
using Warehouse.Core.Entities.Models;

namespace Warehouse.Core.UseCases.Management.Commands
{
    public class SetAlert : AlertEntity, ICommand
    { }

    public class HandleSetAlert : ICommandHandler<SetAlert>
    {
        private readonly IRepository<AlertEntity> _store;
        public HandleSetAlert(IRepository<AlertEntity> store)
        {
            _store = store;
        }

        public async Task<Unit> Handle(SetAlert request, CancellationToken cancellationToken)
        {
            if (!string.IsNullOrEmpty(request.Id) && (await _store.FindAsync(request.Id, cancellationToken)) != null)
            {
                await _store.UpdateAsync(request, cancellationToken);
            }
            else
            {
                await _store.AddAsync(request, cancellationToken);
            }
            return Unit.Value;
        }
    }
}
