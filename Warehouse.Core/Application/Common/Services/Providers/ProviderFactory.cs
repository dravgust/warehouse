using Warehouse.Core.Application.Common.Services.Providers.Default;
using Provider = Warehouse.Core.Application.SystemAdministration.Models.Provider;

namespace Warehouse.Core.Application.Common.Services.Providers
{
    public class ProviderFactory
    {
        private readonly IServiceProvider serviceProvider;

        public ProviderFactory(IServiceProvider serviceProvider)
        {
            this.serviceProvider = serviceProvider;
        }

        public IProviderService GetProviderService(Provider provider)
        {
            return provider switch
            {
                _ => (IProviderService)serviceProvider.GetService(typeof(DefaultProviderService))!
            };
        }

        public IProviderService GetProviderService(string providerName)
        {
            return GetProviderService((Provider) providerName);
        }
    }
}
