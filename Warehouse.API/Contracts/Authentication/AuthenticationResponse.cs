namespace Warehouse.API.Contracts.Authentication
{
    public class AuthenticationResponse
    {
        public string Username { get; set; }
        public string Token { get; set; }
        public long TokenExpirationTime { get; set; }

        public AuthenticationResponse(string username, string jwtToken, long expirationTime)
        {
            Username = username;
            Token = jwtToken;
            TokenExpirationTime = expirationTime;
        }
    }
}
