using Microsoft.Extensions.Logging;
using Vayosoft.Core.Queries;
using Warehouse.Core.Application.Services.Providers;
using Warehouse.Core.Application.UseCases.Administration.Models;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.Application.UseCases.Administration.Queries;

public record GetUserSubscription(string UserName, string ProviderName) : IQuery<UserSubscription>;

public class HandleGetUserSubscription : IQueryHandler<GetUserSubscription, UserSubscription>
{
    private readonly ProviderFactory providerFactory;
    private readonly ILogger<HandleGetUserSubscription> logger;

    public HandleGetUserSubscription(ProviderFactory providerFactory, ILogger<HandleGetUserSubscription> logger)
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
