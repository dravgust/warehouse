namespace Warehouse.API.Contracts.Authentication
{
    public record AuthenticationResponse(
        string Username,
        string Token,
        long TokenExpirationTime);
}
