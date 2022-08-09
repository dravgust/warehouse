using System.Security.Principal;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Entities.Models.Security;

namespace Warehouse.Core.Services
{
    public interface IJwtService
    {
        public string GenerateJwtToken(IUser user, IEnumerable<SecurityRoleEntity> roles);
        public IPrincipal GetPrincipalFromJwtToken(string token);
        public RefreshToken GenerateRefreshToken(string ipAddress);
    }
}
