using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.Administration.Models;

namespace Warehouse.Core.Services
{
    public interface IUserService
    {
        Task<AuthenticateResult> AuthenticateAsync(AuthenticateRequest model, string ipAddress, CancellationToken cancellationToken);
        Task<AuthenticateResult> RefreshTokenAsync(string token, string ipAddress, CancellationToken cancellationToken);
        Task RevokeTokenAsync(string token, string ipAddress, CancellationToken cancellationToken);
        Task<IUser> GetByUserNameAsync(string username, CancellationToken cancellationToken);
    }
}
