﻿using Vayosoft.Commons.Entities;

namespace Warehouse.Core.Domain.Entities.Security
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
        public long? ProviderId { get; set; }
    }

    public class SecurityRolePermissionsEntity : EntityBase<string>
    {
        public virtual string RoleId { get; set; }
        public virtual string ObjectId { get; set; }
        public virtual SecurityPermissions Permissions { get; set; }
    }

    public class UserRoleEntity : EntityBase<string>
    {
        public long UserId { get; set; }
        public string RoleId { get; set; }
    }
}
