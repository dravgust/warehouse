using Warehouse.Core.Entities.Models.Security;

namespace Warehouse.Core.Persistence
{
    public interface IUserRoleStore
    {
        Task<List<SecurityRoleEntity>> GetRolesAsync(IEnumerable<object> providers, CancellationToken cancellationToken = default);

        Task<List<SecurityRoleEntity>> EmbeddedRolesAsync();

        Task<SecurityRoleEntity> GetRoleAsync(string roleId);

        Task<List<SecurityObjectEntity>> GetObjectsAsync();

        Task<List<RolePermissionsDTO>> GetRolePermissionsAsync(string roleId);

        Task<List<RoleDTO>> GetUserRolesAsync(object userId, CancellationToken cancellationToken = default);
    }
}
