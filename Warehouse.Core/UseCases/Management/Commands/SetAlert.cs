using MediatR;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Persistence;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services.Session;

namespace Warehouse.Core.UseCases.Management.Commands
{
    public class SetAlert : AlertEntity, ICommand
    { }

    public class HandleSetAlert : ICommandHandler<SetAlert>
    {
        private readonly IRepository<AlertEntity> _store;
        private readonly ISessionProvider _session;

        public HandleSetAlert(IRepository<AlertEntity> store, ISessionProvider session)
        {
            _store = store;
            _session = session;
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
                var providerId = _session.GetInt64(nameof(IProvider.ProviderId));
                request.ProviderId = providerId ?? 0;
                await _store.AddAsync(request, cancellationToken);
            }
            return Unit.Value;
        }
    }
}
