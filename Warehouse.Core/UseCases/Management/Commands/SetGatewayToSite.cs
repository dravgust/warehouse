using FluentValidation;
using MediatR;
using Vayosoft.Core.Commands;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Persistence;
using Warehouse.Core.UseCases.Management.Models;

namespace Warehouse.Core.UseCases.Management.Commands
{
    public class SetGatewayToSite : GatewayDto, ICommand
    {
        public string SiteId { set; get; }
        public class WarehouseRequestValidator : AbstractValidator<SetGatewayToSite>
        {
            public WarehouseRequestValidator()
            {
                RuleFor(q => q.Name).NotEmpty();
                RuleFor(q => q.SiteId).NotEmpty();
                RuleFor(q => q.MacAddress).MacAddress();
            }
        }
    }

    internal class HandleSetGatewayToSite : ICommandHandler<SetGatewayToSite>
    {
        private readonly WarehouseStore _store;
        private readonly IMapper _mapper;

        public HandleSetGatewayToSite(WarehouseStore store, IMapper mapper)
        {
            _store = store;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(SetGatewayToSite request, CancellationToken cancellationToken)
        {
            var site = await _store.GetAsync<WarehouseSiteEntity>(request.SiteId, cancellationToken);
            var gw = site.Gateways.FirstOrDefault(gw =>
                gw.MacAddress.Equals(request.MacAddress, StringComparison.InvariantCultureIgnoreCase));
            if (gw != null) site.Gateways.Remove(gw);
            site.Gateways.Add(_mapper.Map<Gateway>(request));
            await _store.UpdateAsync(site, cancellationToken);

            return Unit.Value;
        }
    }
}
