using Warehouse.Core.Entities.Models;
using Warehouse.Core.Entities.Models.Security;

namespace Warehouse.Core.Persistence
{
    public interface IUserStore<T> where T : IUser
    {
        Task<T> FindByIdAsync(object userId, CancellationToken cancellationToken = default);

        Task<T> FindByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

        Task<T> FindByNameAsync(string username, CancellationToken cancellationToken = default);

        Task UpdateAsync(IUser user, CancellationToken cancellationToken);
    }
}
