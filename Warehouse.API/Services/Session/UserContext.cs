using System.Security.Principal;
using Warehouse.Core.Entities.Enums;
using Warehouse.Core.Entities.Models.Security;
using Warehouse.Core.Services.Session;
using Warehouse.Core.Utilities;

namespace Warehouse.API.Services.Session
{
    public class UserSession : IUserSession
    {
        public const string SupervisorID = "f6694d71d26e40f5a2abb357177c9bdz";
        public const string AdministratorID = "f6694d71d26e40f5a2abb357177c9bdx";
        public const string SupportID = "f6694d71d26e40f5a2abb357177c9bdt";

        public IPrincipal User { get; }
        public List<RoleDTO> Roles { get; }

        public UserSession(IPrincipal user, List<RoleDTO> roles)
        {
            User = user;
            Roles = roles;
        }

        public bool IsSupervisor =>
            User.Identity.GetUserType() == UserType.Supervisor || User.IsInRole(SupervisorID);
        public bool IsAdministrator =>
            IsSupervisor || User.Identity.GetUserType() == UserType.Administrator || User.IsInRole(AdministratorID);

        public bool HasRole(string role)
        {
            return Roles != null && Roles.Any(r => r.Name.Equals(role, StringComparison.InvariantCultureIgnoreCase));
        }

        public bool HasAnyRole(IEnumerable<string> roles)
        {
            if (IsSupervisor)
                return true;

            if (Roles == null || Roles.Count == 0)
                return false;

            foreach (var role in roles)
            {
                if (HasRole(role))
                    return true;
            }

            return false;
        }

        public bool HasPermission(string objName, SecurityPermissions requiredPermissions)
        {
            if (IsAdministrator)
                return true;

            foreach (var r in Roles)
            {
                if (r.Items == null || r.Items.Count == 0)
                    continue;

                if (r.Items.Any(item => item.ObjectName.Equals(objName, StringComparison.CurrentCultureIgnoreCase)
                                        && item.Permissions.HasFlag(requiredPermissions)))
                    return true;
            }

            return false;
        }
    }
}
