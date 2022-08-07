using System.Security.Principal;
using Warehouse.Core.Entities.Models;

namespace Warehouse.Core.Services
{
    public interface IJwtService
    {
        public string GenerateJwtToken(IUser user);
        public IPrincipal GetPrincipalFromJwtToken(string token);
        public RefreshToken GenerateRefreshToken(string ipAddress);
    }
}
