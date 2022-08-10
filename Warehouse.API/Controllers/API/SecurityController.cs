using Microsoft.AspNetCore.Mvc;
using Warehouse.API.Services.Authorization.Attributes;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Entities.Models.Security;
using Warehouse.Core.Persistence;

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

        [HttpGet("roles")]
        public IActionResult GetRoles()
        {
            return Ok(new { });
        }

        //public ActionResult GetAllowedRoles()
        //{
        //    //UserType
        //    throw new NotImplementedException();
        //    //return Json(Roles.Where(r => r.LID <= (ulong)SessionBE.User.Kind).ToList(), JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult Roles()
        //{
        //    var rl = ViotFactory.DAO.Using(dao => dao.GetAll<SecurityRoleEntity>());
        //    return Json(new { Items = rl, Total = rl.Count }, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult Objs()
        //{
        //    var ol = ViotFactory.DAO.Using(dao => dao.GetAll<SecurityObjectEntity>()).OrderBy(o => o.ObjName).ToList();
        //    return Json(new { Items = ol, Total = ol.Count }, JsonRequestBehavior.AllowGet);
        //}

        //public ActionResult GetRolePermissions(string roleid)
        //{
        //    using (var dao = ViotFactory.DAO)
        //    {
        //        var role = dao.Get<SecurityRoleEntity>(roleid);
        //        if (role == null)
        //            throw new ArgumentException($"Role [{roleid}] not found");

        //        var res = dao.Security.GetRolePermissions(roleid);
        //        var objs = dao.Security.GetObjects();
        //        foreach (var obj in objs)
        //        {
        //            if (!res.Any(p => p.ObjID == obj.ID))
        //                res.Add(new RolePermissionsDTO
        //                {
        //                    ID = null,
        //                    RoleID = roleid,
        //                    ObjID = obj.ID,
        //                    ObjName = obj.ObjName,
        //                    Permissions = ViotPermissions.None
        //                });
        //        }

        //        return Json(new { Role = role, Permissions = res }, JsonRequestBehavior.AllowGet);
        //    }


        //}

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
