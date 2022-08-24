using Warehouse.Core.Entities.Models.Security;

namespace Warehouse.Core.Persistence
{
    public interface IUserRoleStore
    {
        Task<SecurityRolePermissionsEntity> FindRolePermissionsByIdAsync(string roleId, CancellationToken cancellationToken = default);
        Task<SecurityRoleEntity> FindRoleByIdAsync(string roleId, CancellationToken cancellationToken = default);

        Task<List<SecurityObjectEntity>> GetObjectsAsync(CancellationToken cancellationToken = default);
        Task<List<SecurityRoleEntity>> GetRolesAsync(IEnumerable<object> providers, CancellationToken cancellationToken = default);
        Task<List<SecurityRoleEntity>> GetEmbeddedRolesAsync(CancellationToken cancellationToken = default);

        Task<List<RolePermissionsDTO>> GetRolePermissionsAsync(string roleId, CancellationToken cancellationToken = default);
        Task<List<RoleDTO>> GetUserRolesAsync(object userId, CancellationToken cancellationToken = default);

        Task UpdateRoleAsync(SecurityRoleEntity entity, CancellationToken cancellationToken = default);
        Task UpdateRolePermissionsAsync(SecurityRolePermissionsEntity entity, CancellationToken cancellationToken = default);
    }
}
