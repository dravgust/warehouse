using Warehouse.Core.Entities.Models;

namespace Warehouse.Core.Persistence
{
    public interface IUserRepository : IUserStore<UserEntity>, IUserRoleStore
    { }
}
