using Warehouse.Core.UseCases.Administration.Models;

namespace Warehouse.Core.Persistence
{
    public interface IIdentityUserRoleStore
    {
        public Task<List<SecurityRoleEntity>> GetRolesAsync(IEnumerable<object> providers);

        public Task<List<SecurityRoleEntity>> EmbeddedRolesAsync();

        public Task<SecurityRoleEntity> GetRoleAsync(string roleId);

        public Task<List<SecurityObjectEntity>> GetObjectsAsync();

        public dynamic GetRolePermissionsAsync(string roleId);

        public dynamic GetUserRolesAsync(object userId);
    }
}
