using Vayosoft.Core.Persistence;
using Warehouse.Core.Entities.Models;

namespace Warehouse.Core.Persistence
{
    public interface IIdentityUserStore
    {
        public IUnitOfWork UnitOfWork { get; }

        public IIdentityUser GetById(object id);

        public IIdentityUser GetUserByRefreshToken(string token);

        public IIdentityUser? FindUserByNameAsync(string username);
    }
}
