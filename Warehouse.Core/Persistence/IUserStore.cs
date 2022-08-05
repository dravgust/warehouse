using Vayosoft.Core.Persistence;
using Warehouse.Core.Entities.Models;

namespace Warehouse.Core.Persistence
{
    public interface IUserStore<T> where T : IUser
    {
        public IUnitOfWork UnitOfWork { get; }

        public Task<T> FindByIdAsync(object userId, CancellationToken cancellationToken = default);

        public Task<T> FindByRefreshTokenAsync(string refreshToken, CancellationToken cancellationToken = default);

        public Task<T> FindByNameAsync(string username, CancellationToken cancellationToken = default);

        public Task UpdateAsync(IUser user, CancellationToken cancellationToken);
    }
}
