using Warehouse.Core.Entities.Models.Security;

namespace Warehouse.Core.Persistence
{
    public interface IUserRoleStore
    {
        Task<List<SecurityRoleEntity>> GetRolesAsync(IEnumerable<object> providers, CancellationToken cancellationToken = default);

        Task<List<SecurityRoleEntity>> EmbeddedRolesAsync(CancellationToken cancellationToken = default);

        Task<SecurityRoleEntity> GetRoleAsync(string roleId, CancellationToken cancellationToken = default);

        Task<List<SecurityObjectEntity>> GetObjectsAsync(CancellationToken cancellationToken = default);

        Task<List<RolePermissionsDTO>> GetRolePermissionsAsync(string roleId, CancellationToken cancellationToken = default);

        Task<List<RoleDTO>> GetUserRolesAsync(object userId, CancellationToken cancellationToken = default);
    }
}
