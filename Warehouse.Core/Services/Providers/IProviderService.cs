using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.Administration.Models;

namespace Warehouse.Core.Services.Providers
{
    public interface IProviderService
    {
        public Task Send(string notification);

        Task<UserSubscription> GetUserSubscription(UserEntity user);
    }
}
