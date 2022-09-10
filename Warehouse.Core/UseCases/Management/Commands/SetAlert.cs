using MediatR;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Persistence;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services;
using Warehouse.Core.Services.Security;

namespace Warehouse.Core.UseCases.Management.Commands
{
    public class SetAlert : AlertEntity, ICommand
    { }

    public class HandleSetAlert : ICommandHandler<SetAlert>
    {
        private readonly IRepositoryBase<AlertEntity> _store;
        private readonly IUserContext _userContext;

        public HandleSetAlert(IRepositoryBase<AlertEntity> store, IUserContext userContext)
        {
            _store = store;
            _userContext = userContext;
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
                var providerId = _userContext.User.Identity.GetProviderId();
                request.ProviderId = providerId;
                await _store.AddAsync(request, cancellationToken);
            }
            return Unit.Value;
        }
    }
}
