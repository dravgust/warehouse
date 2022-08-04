using FluentValidation;
using MediatR;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Persistence;

namespace Warehouse.Core.UseCases.Management.Commands
{
    public class RemoveGatewayFromSite : ICommand
    {
        public string SiteId { set; get; }
        public string MacAddress { set; get; }
        public class WarehouseRequestValidator : AbstractValidator<RemoveGatewayFromSite>
        {
            public WarehouseRequestValidator()
            {
                RuleFor(q => q.SiteId).NotEmpty();
                RuleFor(q => q.MacAddress).MacAddress();
            }
        }
    }

    internal class HandleRemoveGatewayFromSite : ICommandHandler<RemoveGatewayFromSite>
    {
        private readonly WarehouseStore _store;

        public HandleRemoveGatewayFromSite(WarehouseStore store)
        {
            _store = store;
        }

        public async Task<Unit> Handle(RemoveGatewayFromSite request, CancellationToken cancellationToken)
        {
            var site = await _store.GetAsync<WarehouseSiteEntity>(request.SiteId, cancellationToken);
            var gw = site.Gateways.FirstOrDefault(gw =>
                gw.MacAddress.Equals(request.MacAddress, StringComparison.InvariantCultureIgnoreCase));
            if (gw != null) site.Gateways.Remove(gw);
            await _store.UpdateAsync(site, cancellationToken);

            return Unit.Value;
        }
    }
}
