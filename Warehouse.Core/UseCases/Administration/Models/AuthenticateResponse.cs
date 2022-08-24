namespace Warehouse.Core.UseCases.Administration.Models
{
    public class AuthenticateResponse
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public long TokenExpirationTime { get; set; }

        public AuthenticateResponse(string username, string jwtToken, long expirationTime)
        {
            Username = username;
            Token = jwtToken;
            TokenExpirationTime = expirationTime;
        }
    }
}
