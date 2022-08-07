using Warehouse.Core.Entities.Models;
using Warehouse.Core.Entities.ValueObjects;

namespace Warehouse.Core.Services
{
    public interface IJwtService
    {
        public string GenerateJwtToken(IUser user);
        public UserContext ValidateJwtToken(string token);
        public RefreshToken GenerateRefreshToken(string ipAddress);
    }
}
