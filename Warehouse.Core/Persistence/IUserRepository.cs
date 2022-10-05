using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.Persistence
{
    public interface IUserRepository : IUserStore<UserEntity>, IUserRoleStore
    { }
}
