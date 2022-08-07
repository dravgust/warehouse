
using Warehouse.Core.Entities.Enums;

namespace Warehouse.Core.Entities.ValueObjects
{
    public class UserContext
    {
        public long? UserId { get; set; }
        public UserType UserType { get; set; }
        public long? ProviderId { get; set; }
    }
}
