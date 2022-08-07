using System.Collections.ObjectModel;
using Warehouse.Core.Entities.Models.Security;

namespace Warehouse.Core.Persistence
{
    public interface IUserRoleStore
    {
        public Task<List<SecurityRoleEntity>> GetRolesAsync(IEnumerable<object> providers);

        public Task<List<SecurityRoleEntity>> EmbeddedRolesAsync();

        public Task<SecurityRoleEntity> GetRoleAsync(string roleId);

        public Task<List<SecurityObjectEntity>> GetObjectsAsync();

        public Task<List<RolePermissionsDTO>> GetRolePermissionsAsync(string roleId);

        public Task<List<RoleDTO>> GetUserRolesAsync(object userId);
    }
}
