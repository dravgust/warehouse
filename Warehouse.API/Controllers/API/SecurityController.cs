using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Utilities;
using Warehouse.API.Services.Authorization.Attributes;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Entities.Models.Security;
using Warehouse.Core.Exceptions;
using Warehouse.Core.Persistence;
using Warehouse.Core.Services;
using Warehouse.Core.Utilities;

namespace Warehouse.API.Controllers.API
{
    [Route("api/[controller]")]
    [PermissionAuthorization("USER", SecurityPermissions.Grant)]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly IUserStore<UserEntity> _userStore;
        private readonly IUserContext _userContext;
        private readonly ILogger<SecurityController> _logger;

        public SecurityController(
            IUserStore<UserEntity> userStore,
            IUserContext userContext, 
            ILogger<SecurityController> logger)
        {
            _userStore = userStore;
            _userContext = userContext;
            _logger = logger;
        }

        [HttpGet("user-roles")]
        public async Task<IActionResult> GetUserRoles(CancellationToken token)
        {
            var items = new List<RoleDTO>();
            if (_userStore is IUserRoleStore store)
            {
                var userId = HttpContext.User.Identity!.GetUserId();
                items.AddRange(await store.GetUserRolesAsync(userId, token));
            }

            return Ok(new { items, Total = items.Count });
        }

        [HttpGet("roles")]
        public async Task<IActionResult> GetRoles(CancellationToken token)
        {
            var items = new List<SecurityRoleEntity>();
            if (_userStore is IUserRoleStore store)
            {
                var providerId = HttpContext.User.Identity?.GetProviderId() ?? 0;
                items.AddRange(await store.GetRolesAsync(new object[] { providerId }, token)!);
            }

            return Ok(new { items, Total = items.Count });
        }

        [HttpGet("objects")]
        public async Task<IActionResult> GetObjects(CancellationToken token)
        {
            var items = new List<SecurityObjectEntity>();
            if (_userStore is IUserRoleStore store)
            {
                items.AddRange(await store.GetObjectsAsync(token));
            }

            return Ok(new { items, Total = items.Count });
        }

        [HttpGet("permissions/{roleId}")]
        public async Task<IActionResult> GetRolePermissions(string roleId, CancellationToken token)
        {
            Guard.NotEmpty(roleId, nameof(roleId));

            SecurityRoleEntity role = null;
            var permissions = new List<RolePermissionsDTO>();
            if (_userStore is IUserRoleStore store)
            {
                role = await store.GetRoleAsync(roleId, token);
                if (role == null)
                    return NotFound(roleId);

                permissions = await store.GetRolePermissionsAsync(roleId, token);
                var objects = await store.GetObjectsAsync(token);
                foreach (var obj in objects)
                {
                    if (permissions.All(p => p.ObjectId != obj.Id))
                        permissions.Add(new RolePermissionsDTO
                        {
                            Id = null,
                            RoleId = roleId,
                            ObjectId = obj.Id,
                            ObjectName = obj.Name,
                            Permissions = SecurityPermissions.None
                        });
                }
            }

            return Ok(new { role, permissions });
        }

        [HttpPost("roles/save")]
        public async Task<IActionResult> SaveRole(SecurityRoleEntity role, CancellationToken token)
        {
            if (_userStore is IUserRoleStore store)
            {
                try
                {
                    await _userContext.LoadSessionAsync();

                    if (!_userContext.IsAdministrator)
                        throw new NotEnoughPermissionsException();

                    if (!_userContext.IsSupervisor)
                        role.ProviderId = _userContext.User.Identity?.GetProviderId();

                    if (!string.IsNullOrEmpty(role.Id))
                    {
                        var old = await store.GetRoleAsync(role.Id, token);
                        if (old == null)
                            throw new ArgumentException("Role not found by Id");

                        if (!old.Name.Equals(role.Name) || !string.Equals(old.Description, role.Description))
                            await store.UpdateAsync(role, token);
                    }
                    else
                    {
                        await store.UpdateAsync(role, token);
                    }
                }
                catch (Exception e)
                {
                    _logger.LogError($"{e.Message}\r\n{e.StackTrace}");
                }
            }

            return Ok();
        }

        [HttpPost("permissions/save")]
        public async Task<IActionResult> SavePermissions(List<RolePermissionsDTO> permissions, CancellationToken token)
        {
            try
            {
                var roles = new List<SecurityRoleEntity>();

                if (_userStore is IUserRoleStore store)
                {
                    foreach (var p in permissions)
                    {
                        var role = roles.FirstOrDefault(r => r.Id == p.RoleId);
                        if (role == null)
                        {
                            role = await store.GetRoleAsync(p.RoleId, token);
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
                            _logger.LogError( $"SavePermissions: User: {_userContext.User.Identity?.Name}" +
                                              $" without supervisor rights try edit embedded role [{role.Name}]");
                            continue;
                        }

                        SecurityRolePermissionsEntity rp = null;
                        if (!string.IsNullOrEmpty(p.Id))
                            rp = await store.GetRolePermissionAsync(p.Id, token);

                        if (rp != null)
                        {
                            rp.Permissions = p.Permissions;
                            await store.UpdateAsync(rp, token);
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
                            await store.UpdateAsync(rp, token);
                        }
                    }
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"{e.Message}\r\n{e.StackTrace}");
            }
            return Ok();
        }
    }
}
