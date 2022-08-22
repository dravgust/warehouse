using MediatR;
using Microsoft.Extensions.Logging;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Entities.Models.Security;
using Warehouse.Core.Persistence;
using Warehouse.Core.Services;

namespace Warehouse.Core.UseCases.Administration.Commands
{
    public class SavePermissions : List<RolePermissionsDTO>, ICommand
    { }

    public class HandleSavePermissions : ICommandHandler<SavePermissions>
    {
        private readonly IUserStore<UserEntity> _userStore;
        private readonly IUserContext _userContext;
        private readonly ILogger<SavePermissions> _logger;

        public HandleSavePermissions(IUserStore<UserEntity> userStore, IUserContext userContext, ILogger<SavePermissions> logger)
        {
            _userStore = userStore;
            _userContext = userContext;
            _logger = logger;
        }
        public async Task<Unit> Handle(SavePermissions command, CancellationToken cancellationToken)
        {
            try
            {
                var roles = new List<SecurityRoleEntity>();

                if (_userStore is IUserRoleStore store)
                {
                    foreach (var p in command)
                    {
                        var role = roles.FirstOrDefault(r => r.Id == p.RoleId);
                        if (role == null)
                        {
                            role = await store.FindRoleByIdAsync(p.RoleId, cancellationToken);
                            if (role == null)
                            {
                                _logger.LogError($"SavePermissions: Role not found [{p.RoleId}] for User: {_userContext.User.Identity?.Name}");
                                continue;
                            }
                            roles.Add(role);
                        }

                        await _userContext.LoadSessionAsync();
                        if (!_userContext.IsSupervisor && role.ProviderId == null)
                        {
                            _logger.LogError($"SavePermissions: User: {_userContext.User.Identity?.Name}" +
                                              $" without supervisor rights try edit embedded role [{role.Name}]");
                            continue;
                        }

                        SecurityRolePermissionsEntity rp = null;
                        if (!string.IsNullOrEmpty(p.Id))
                            rp = await store.FindRolePermissionsByIdAsync(p.Id, cancellationToken);

                        if (rp != null)
                        {
                            rp.Permissions = p.Permissions;
                            await store.UpdateRolePermissionsAsync(rp, cancellationToken);
                        }
                        else
                        {
                            rp = new SecurityRolePermissionsEntity
                            {
                                Id = GuidGenerator.New().ToString("N"),
                                ObjectId = p.ObjectId,
                                RoleId = p.RoleId,
                                Permissions = p.Permissions
                            };
                            await store.UpdateRolePermissionsAsync(rp, cancellationToken);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message}\r\n{e.StackTrace}");
            }

            return Unit.Value;
        }
    }
}
