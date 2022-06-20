using System.Text.Json.Serialization;
using Vayosoft.WebAPI.Entities;

namespace Vayosoft.WebAPI.Models
{
    public class AuthenticateResponse
    {
        public object Id { get; set; }
        public string Username { get; set; }
        public string Email { get; set; }
        public string Token { get; set; }

        [JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }

        public AuthenticateResponse(IIdentityUser user, string jwtToken, string refreshToken)
        {
            Id = user.Id;
            Username = user.Username;
            Username = user.Email;
            Token = jwtToken;
            RefreshToken = refreshToken;
        }
    }
}
