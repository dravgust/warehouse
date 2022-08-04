using Vayosoft.Core.SharedKernel.Entities;

namespace Warehouse.Core.UseCases.Administration.Models
{
    public class SecurityObjectEntity : EntityBase<string>
    {
        public string ObjName { get; set; }
        public string ObjDesc { get; set; }
    }

    public class SecurityRoleEntity : EntityBase<string>
    {
        public virtual string RoleName { get; set; }
        public virtual string RoleDesc { get; set; }
        public ulong? ProviderID { get; set; }
    }

    public class SecurityRolePermissionsEntity : EntityBase<string>
    {
        public virtual string RoleID { get; set; }
        public virtual string ObjID { get; set; }
        public virtual WarehousePermissions Permissions { get; set; }
    }

    public class UserRoleEntity : EntityBase<string>
    {
        public ulong UserID { get; set; }
        public string RoleID { get; set; }
    }

    [Flags]
    public enum WarehousePermissions
    {
        None = 0,
        View = 1,
        Add = 2,
        Edit = 4,
        Delete = 8,
        Execute = 16,
        Grant = 32
    }
}
