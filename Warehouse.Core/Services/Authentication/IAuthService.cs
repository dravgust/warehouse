namespace Warehouse.Core.Services.Authentication
{
    public interface IAuthService
    {
        Task<AuthResult> AuthenticateAsync(string username, string password, string ipAddress, CancellationToken cancellationToken);
        Task<AuthResult> RefreshTokenAsync(string token, string ipAddress, CancellationToken cancellationToken);
        Task RevokeTokenAsync(string token, string ipAddress, CancellationToken cancellationToken);
    }
}
