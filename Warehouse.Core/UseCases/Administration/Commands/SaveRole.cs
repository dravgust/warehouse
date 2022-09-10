using MediatR;
using Microsoft.Extensions.Logging;
using Vayosoft.Core.Commands;
using Vayosoft.Core.SharedKernel.Exceptions;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Entities.Models.Security;
using Warehouse.Core.Exceptions;
using Warehouse.Core.Persistence;
using Warehouse.Core.Services;
using Warehouse.Core.Services.Security;

namespace Warehouse.Core.UseCases.Administration.Commands
{
    public class SaveRole : SecurityRoleEntity, ICommand
    { }

    public class HandleSaveRole : ICommandHandler<SaveRole>
    {
        private readonly IUserStore<UserEntity> _userStore;
        private readonly IUserContext _userContext;
        private readonly ILogger<HandleSaveRole> _logger;

        public HandleSaveRole(IUserStore<UserEntity> userStore, IUserContext userContext, ILogger<HandleSaveRole> logger)
        {
            _userStore = userStore;
            _userContext = userContext;
            _logger = logger;
        }
        public async Task<Unit> Handle(SaveRole command, CancellationToken cancellationToken)
        {
            if (_userStore is IUserRoleStore store)
            {
                try
                {
                    await _userContext.LoadSessionAsync();

                    if (!_userContext.IsAdministrator)
                        throw new NotEnoughPermissionsException();

                    if (!_userContext.IsSupervisor)
                        command.ProviderId = _userContext.User.Identity?.GetProviderId();

                    if (!string.IsNullOrEmpty(command.Id))
                    {
                        var old = await store.FindRoleByIdAsync(command.Id, cancellationToken);
                        if (old == null)
                            throw new EntityNotFoundException(nameof(SecurityRoleEntity), command.Id);

                        if (!old.Name.Equals(command.Name) || !string.Equals(old.Description, command.Description))
                            await store.UpdateSecurityRoleAsync(command, cancellationToken);
                    }
                    else
                    {
                        await store.UpdateSecurityRoleAsync(command, cancellationToken);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError($"{e.Message}\r\n{e.StackTrace}");
                }
            }

            return Unit.Value;
        }
    }
}
