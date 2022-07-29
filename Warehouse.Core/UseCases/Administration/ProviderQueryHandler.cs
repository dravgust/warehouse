using Microsoft.Extensions.Logging;
using Vayosoft.Core.Queries;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services.Providers;
using Warehouse.Core.UseCases.Administration.Models;
using Warehouse.Core.UseCases.Administration.Queries;

namespace Warehouse.Core.UseCases.Administration
{
    public class ProviderQueryHandler : IQueryHandler<GetUserSubscription, UserSubscription>
    {
        private readonly ProviderFactory providerFactory;
        private readonly ILogger<ProviderQueryHandler> logger;

        public ProviderQueryHandler(ProviderFactory providerFactory, ILogger<ProviderQueryHandler> logger)
        {
            this.providerFactory = providerFactory;
            this.logger = logger;
        }

        public async Task<UserSubscription> Handle(GetUserSubscription request, CancellationToken cancellationToken)
        {
            var (userName, providerName) = request;
            var provider = providerFactory.GetProviderService(providerName);
            return await provider.GetUserSubscription(new UserEntity(userName));
        }
    }
}
