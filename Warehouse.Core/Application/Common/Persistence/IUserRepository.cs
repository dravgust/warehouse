using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.Application.Common.Persistence
{
    public interface IUserRepository : IUserStore<UserEntity>, IUserRoleStore
    { }
}
