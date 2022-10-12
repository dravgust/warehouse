using Warehouse.Core.Domain.Entities.Security;

namespace Warehouse.Core.Application.SystemAdministration.Models
{
    public class RolePermissions
    {
        public RolePermissions(SecurityRoleEntity role, List<RolePermissionsDTO> permissions)
        {
            Role = role;
            Permissions = permissions;
        }

        public SecurityRoleEntity Role { get; set; }

        public List<RolePermissionsDTO> Permissions { get; set; }
    }
}
