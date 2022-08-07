using MediatR;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Persistence;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Entities.ValueObjects;

namespace Warehouse.Core.UseCases.Management.Commands
{
    public class SetAlert : AlertEntity, ICommand
    { }

    public class HandleSetAlert : ICommandHandler<SetAlert>
    {
        private readonly IRepository<AlertEntity> _store;
        private readonly IUserIdentity _identity;

        public HandleSetAlert(IRepository<AlertEntity> store, IUserIdentity identity)
        {
            _store = store;
            _identity = identity;
        }

        public async Task<Unit> Handle(SetAlert request, CancellationToken cancellationToken)
        {
            AlertEntity entity;
            if (!string.IsNullOrEmpty(request.Id) && (entity = await _store.FindAsync(request.Id, cancellationToken)) != null)
            {
                entity.Name = request.Name;
                entity.CheckPeriod = request.CheckPeriod;
                entity.Enabled = request.Enabled;
                await _store.UpdateAsync(entity, cancellationToken);
            }
            else
            {
                request.ProviderId = _identity.ProviderId ?? 0;
                await _store.AddAsync(request, cancellationToken);
            }
            return Unit.Value;
        }
    }
}
