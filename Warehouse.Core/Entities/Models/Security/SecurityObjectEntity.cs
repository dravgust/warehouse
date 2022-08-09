using Vayosoft.Core.SharedKernel.Entities;

namespace Warehouse.Core.Entities.Models.Security
{
    public class SecurityObjectEntity : EntityBase<string>
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public class SecurityRoleEntity : EntityBase<string>
    {
        public virtual string Name { get; set; }
        public virtual string Description { get; set; }
        public ulong? ProviderId { get; set; }
    }

    public class SecurityRolePermissionsEntity : EntityBase<string>
    {
        public virtual string RoleId { get; set; }
        public virtual string ObjectId { get; set; }
        public virtual SecurityPermissions Permissions { get; set; }
    }

    public class UserRoleEntity : EntityBase<string>
    {
        public ulong UserId { get; set; }
        public string RoleId { get; set; }
    }
}
