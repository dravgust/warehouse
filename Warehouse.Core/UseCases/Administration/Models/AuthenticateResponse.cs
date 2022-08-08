using System.Text.Json.Serialization;
using Warehouse.Core.Entities.Models;

namespace Warehouse.Core.UseCases.Administration.Models
{
    public class AuthenticateResponse
    {
        public object Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }
        public long TokenExpirationTime { get; set; }

        [JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }

        public AuthenticateResponse(IUser user, string jwtToken, string refreshToken, DateTime expirationTime)
        {
            Id = user.Id;
            Username = user.Username;
            Email = user.Email;
            Token = jwtToken;
            TokenExpirationTime = ((DateTimeOffset) expirationTime).ToUnixTimeSeconds();
            RefreshToken = refreshToken;
        }
    }
}
