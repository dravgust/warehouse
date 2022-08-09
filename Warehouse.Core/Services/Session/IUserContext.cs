using Warehouse.Core.Entities.Models.Security;

namespace Warehouse.Core.Services.Session
{
    public interface IUserSession
    {
        bool HasRole(string role);
        bool HasAnyRole(IEnumerable<string> roles);
        bool HasPermission(string objName, SecurityPermissions requiredPermissions);
    }
}
