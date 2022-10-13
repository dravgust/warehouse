using FluentValidation;
using MediatR;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.Application.SiteManagement.Commands
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
        private readonly IRepository<WarehouseSiteEntity> _repository;

        public HandleRemoveGatewayFromSite(IRepository<WarehouseSiteEntity> repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(RemoveGatewayFromSite request, CancellationToken cancellationToken)
        {
            var site = await _repository.GetAsync(request.SiteId, cancellationToken);
            var gw = site.Gateways.FirstOrDefault(gw =>
                gw.MacAddress.Equals(request.MacAddress, StringComparison.InvariantCultureIgnoreCase));
            if (gw != null) site.Gateways.Remove(gw);
            await _repository.UpdateAsync(site, cancellationToken);

            return Unit.Value;
        }
    }
}
