using MediatR;
using Vayosoft.Core.Commands;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Persistence;

namespace Warehouse.Core.UseCases.Management.Commands
{
    public class SetAlert : AlertEntity, ICommand
    { }

    public class HandleSetAlert : ICommandHandler<SetAlert>
    {
        private readonly WarehouseStore _store;
        public HandleSetAlert(WarehouseStore store)
        {
            _store = store;
        }

        public async Task<Unit> Handle(SetAlert request, CancellationToken cancellationToken)
        {
            AlertEntity entity;
            if (!string.IsNullOrEmpty(request.Id) && (entity = await _store.FindAsync<AlertEntity>(request.Id, cancellationToken)) != null)
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
