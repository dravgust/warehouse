using Warehouse.Core.Entities.Models;

namespace Warehouse.Core.Entities.Enums
{
    public static class CustomClaimTypes
    {
        public const string UserType = nameof(IUser.Type);
        public const string ProviderId = nameof(IProvider.ProviderId);
    }
}
