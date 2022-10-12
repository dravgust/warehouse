using Warehouse.Core.Application.SystemAdministration.Models;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.Application.Common.Services.Providers
{
    public interface IProviderService
    {
        public Task Send(string notification);

        Task<UserSubscription> GetUserSubscription(UserEntity user);
    }
}
