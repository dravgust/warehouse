using Warehouse.Core.Domain.Entities.Identity;

namespace Warehouse.Core.Domain.Enums
{
    public static class CustomClaimTypes
    {
        public const string UserType = nameof(IUser.Type);
        public const string ProviderId = nameof(IProvider.ProviderId);
    }
}
