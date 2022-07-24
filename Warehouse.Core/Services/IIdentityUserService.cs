using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.Administration.Models;

namespace Warehouse.Core.Services
{
    public interface IIdentityUserService
    {
        Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest model, string ipAddress, CancellationToken cancellationToken);
        Task<AuthenticateResponse> RefreshTokenAsync(string token, string ipAddress, CancellationToken cancellationToken);
        Task RevokeTokenAsync(string token, string ipAddress, CancellationToken cancellationToken);
        Task<IIdentityUser> GetByIdAsync(object id, CancellationToken cancellationToken);
    }
}
