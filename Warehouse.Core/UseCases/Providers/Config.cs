using MediatR;
using Microsoft.Extensions.DependencyInjection;
using Warehouse.Core.UseCases.Administration.Models;
using Warehouse.Core.UseCases.Administration.Queries;

namespace Warehouse.Core.UseCases.Providers
{
    public static class Config
    {
        public static void AddProviderHandlers(this IServiceCollection services)
        {
            services.AddScoped<IRequestHandler<GetUserSubscription, UserSubscription>, ProviderQueryHandler>();
        }
    }
}
