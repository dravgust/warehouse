using MediatR;
using Vayosoft.Commands;
using Vayosoft.Persistence;
using Warehouse.Core.Application.Common.Services;
using Warehouse.Core.Application.Common.Services.Security;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.Application.SiteManagement.Commands
{
    public class SetAlert : AlertEntity, ICommand
    { }

    public class HandleSetAlert : ICommandHandler<SetAlert>
    {
        private readonly IRepository<AlertEntity> _store;
        private readonly IUserContext _userContext;

        public HandleSetAlert(IRepository<AlertEntity> store, IUserContext userContext)
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
