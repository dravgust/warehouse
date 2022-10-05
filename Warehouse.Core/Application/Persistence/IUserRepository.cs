using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.Application.Persistence
{
    public interface IUserRepository : IUserStore<UserEntity>, IUserRoleStore
    { }
}
