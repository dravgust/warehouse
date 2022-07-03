using Vayosoft.Core.Queries;
using Warehouse.Core.UseCases.Administration.Models;

namespace Warehouse.Core.UseCases.Administration.Queries
{
    public record GetUserSubscription(string UserName, string ProviderName) : IQuery<UserSubscription>;
}
