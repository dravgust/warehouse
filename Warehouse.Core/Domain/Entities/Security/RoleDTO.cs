namespace Warehouse.Core.Domain.Entities.Security
{
    public class RolePermissionsDTO : SecurityRolePermissionsEntity
    {
        public string ObjectName { get; set; }
    }

    public class RoleDTO : SecurityRoleEntity
    {
        public RoleDTO() { }

        public RoleDTO(SecurityRoleEntity r)
        {
            Id = r.Id;
            Name = r.Name;
            Description = r.Description;
            ProviderId = r.ProviderId;
        }

        public List<RolePermissionsDTO> Items { get; set; }
    }
}
