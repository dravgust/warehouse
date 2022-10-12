﻿namespace Warehouse.Core.Application.Common.Services.Authentication
{
    public interface IAuthenticationService
    {
        Task<AuthenticationResult> AuthenticateAsync(string username, string password, string ipAddress, CancellationToken cancellationToken);
        Task<AuthenticationResult> RefreshTokenAsync(string token, string ipAddress, CancellationToken cancellationToken);
        Task RevokeTokenAsync(string token, string ipAddress, CancellationToken cancellationToken);
    }
}
