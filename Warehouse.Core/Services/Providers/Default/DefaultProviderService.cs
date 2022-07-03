using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.Administration.Models;

namespace Warehouse.Core.Services.Providers.Default
{
    public class DefaultProviderService : IProviderService
    {
        protected IServiceProvider Services { get; }

        public DefaultProviderService(IServiceProvider services)
        {
            this.Services = services;
        }

        public Task Send(string notification)
        {
            return Task.CompletedTask;
        }

        public Task<UserSubscription> GetUserSubscription(UserEntity user)
        {
            throw new NotImplementedException();
        }
    }
}
