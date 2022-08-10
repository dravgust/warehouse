using System.Text.Json.Serialization;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Entities.Models.Security;

namespace Warehouse.Core.UseCases.Administration.Models
{
    public class AuthenticateResponse
    {
        public IUser User { get; set; }
        [JsonIgnore]
        public IList<RoleDTO> Roles { get; set; }
        public string Token { get; set; }
        public long TokenExpirationTime { get; set; }

        [JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }

        public AuthenticateResponse(IUser user, IList<RoleDTO> roles, string jwtToken, string refreshToken, DateTime expirationTime)
        {
            User = user;
            Roles = roles;
            Token = jwtToken;
            TokenExpirationTime = ((DateTimeOffset) expirationTime).ToUnixTimeSeconds();
            RefreshToken = refreshToken;
        }
    }
}
