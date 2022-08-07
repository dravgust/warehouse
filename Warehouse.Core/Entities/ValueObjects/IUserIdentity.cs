using Warehouse.Core.Entities.Enums;

namespace Warehouse.Core.Entities.ValueObjects
{
    public interface IUserIdentity
    {
        long? UserId { get; set; }
        UserType UserType { get; set; }
        long? ProviderId { get; set; }
    }
}
