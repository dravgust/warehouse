using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.Administration.Models;

namespace Warehouse.Core.Services
{
    public interface IUserService
    {
        Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest model, string ipAddress, CancellationToken cancellationToken);
        Task<AuthenticateResponse> RefreshTokenAsync(string token, string ipAddress, CancellationToken cancellationToken);
        Task RevokeTokenAsync(string token, string ipAddress, CancellationToken cancellationToken);
        Task<IUser> GetByIdAsync(object id, CancellationToken cancellationToken);
    }
}
