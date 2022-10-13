using System.Security.Principal;
using Warehouse.Core.Domain.Entities.Identity;
using Warehouse.Core.Domain.Entities.Security;
using Warehouse.Core.Domain.ValueObjects;

namespace Warehouse.Core.Application.Common.Services
{
    public interface IJwtService
    {
        public string GenerateJwtToken(IUser user, IEnumerable<SecurityRoleEntity> roles);
        public IPrincipal GetPrincipalFromJwtToken(string token);
        public RefreshToken GenerateRefreshToken(string ipAddress);
    }
}
