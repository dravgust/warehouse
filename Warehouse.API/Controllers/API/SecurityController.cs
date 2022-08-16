using Microsoft.AspNetCore.Mvc;
using Vayosoft.Core.Utilities;
using Warehouse.API.Services.Authorization.Attributes;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Entities.Models.Security;
using Warehouse.Core.Persistence;
using Warehouse.Core.Utilities;

namespace Warehouse.API.Controllers.API
{
    [Route("api/[controller]")]
    [PermissionAuthorization("USER", SecurityPermissions.Grant)]
    [ApiController]
    public class SecurityController : ControllerBase
    {
        private readonly IUserStore<UserEntity> _userStore;

        public SecurityController(IUserStore<UserEntity> userStore)
        {
            _userStore = userStore;
        }

        [HttpGet("user-roles")]
        public async Task<IActionResult> GetUserRoles(CancellationToken token)
        {
            var items = new List<RoleDTO>();
            if (_userStore is IUserRoleStore store)
            {
                var userId = HttpContext.User.Identity?.GetUserId();
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

        //public ActionResult SaveRole(SecurityRoleEntity role)
        //{
        //    var res = false;
        //    try
        //    {
        //        if (!SessionBE.IsAdministrator)
        //            throw new NotEnoughPermissionsException();

        //        if (!SessionBE.IsSupervisor)
        //            role.ProviderID = SessionBE.Provider.ID;

        //        using (var dao = ViotFactory.DAO)
        //        {
        //            if (role.HasID)
        //            {
        //                var old = dao.Get<SecurityRoleEntity>(role.ID);
        //                if (old == null)
        //                    throw new ArgumentException("Role not found by ID");

        //                if (!old.RoleName.Equals(old.RoleName) || !string.Equals(old.RoleDesc, role.RoleDesc))
        //                    dao.UpdateAndFlush(role);
        //            }
        //            else
        //            {
        //                dao.AddAndFlush(role);
        //            }
        //        }


        //        res = true;
        //    }
        //    catch (Exception e)
        //    {
        //        ViotLogger.Error(e);
        //    }
        //    return Json(new { Status = res }, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult SavePermissions(List<RolePermissionsDTO> permissions)
        //{
        //    var count = 0;
        //    try
        //    {
        //        var roles = new List<SecurityRoleEntity>();

        //        using (var dao = ViotFactory.DAO)
        //        {
        //            foreach (var p in permissions)
        //            {
        //                var role = roles.FirstOrDefault(r => r.ID == p.RoleID);
        //                if (role == null)
        //                {
        //                    role = dao.Get<SecurityRoleEntity>(p.RoleID);
        //                    if (role == null)
        //                    {
        //                        ViotLogger.Error(SessionBE.User.Username, $"SavePermissions: Role not found [{p.RoleID}]");
        //                        continue;
        //                    }

        //                    roles.Add(role);
        //                }

        //                if (!SessionBE.IsSupervisor && role.ProviderID == null)
        //                {
        //                    ViotLogger.Error(SessionBE.User.Username, $"SavePermissions: User without supervisor rights try edit embedded role [{role.RoleName}]");
        //                    continue;
        //                }

        //                SecurityRolePermissionsEntity rp = null;
        //                if (p.HasID)
        //                    rp = dao.Get<SecurityRolePermissionsEntity>(p.ID);

        //                if (rp != null)
        //                {
        //                    rp.Permissions = p.Permissions;
        //                    dao.UpdateAndFlush(rp);
        //                }
        //                else
        //                {
        //                    rp = new SecurityRolePermissionsEntity
        //                    {
        //                        ID = Utils.CreateUID(),
        //                        ObjID = p.ObjID,
        //                        RoleID = p.RoleID,
        //                        Permissions = p.Permissions
        //                    };
        //                    dao.AddAndFlush(rp);
        //                }

        //                count++;
        //            }
        //        }
        //    }
        //    catch (Exception e)
        //    {
        //        ViotLogger.Error(e);
        //    }
        //    return Json(new { Status = (count > 0) }, JsonRequestBehavior.AllowGet);
        //}
    }
}
