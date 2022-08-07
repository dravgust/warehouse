using Warehouse.Core.Entities.Enums;
using Warehouse.Core.Entities.ValueObjects;

namespace Warehouse.API.Services.Authorization
{
    public class UserIdentity : IUserIdentity
    {
        public long? UserId { get; set; }
        public UserType UserType { get; set; }
        public long? ProviderId { get; set; }
    }
}
