using Vayosoft.WebAPI.Entities;

namespace Vayosoft.WebAPI.Middlewares.Jwt
{
    public interface IJwtUtils
    {
        public string GenerateJwtToken(IIdentityUser user);
        public int? ValidateJwtToken(string token);
        public RefreshToken GenerateRefreshToken(string ipAddress);
    }
}
