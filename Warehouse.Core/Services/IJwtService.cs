using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.Administration.ValueObjects;

namespace Warehouse.Core.Services
{
    public interface IJwtService
    {
        public string GenerateJwtToken(IUser user);
        public IdentityContext ValidateJwtToken(string token);
        public RefreshToken GenerateRefreshToken(string ipAddress);
    }
}
