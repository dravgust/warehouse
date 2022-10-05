using Warehouse.Core.Application.UseCases.Administration.Models;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.Application.Services.Providers
{
    public interface IProviderService
    {
        public Task Send(string notification);

        Task<UserSubscription> GetUserSubscription(UserEntity user);
    }
}
