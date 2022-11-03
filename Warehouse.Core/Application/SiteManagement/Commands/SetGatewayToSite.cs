﻿using FluentValidation;
using MediatR;
using Vayosoft.Commands;
using Vayosoft.Persistence;
using Vayosoft.Commons;
using Vayosoft.Utilities;
using Warehouse.Core.Application.SiteManagement.Models;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.Application.SiteManagement.Commands
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
        private readonly IRepository<WarehouseSiteEntity> _repository;
        private readonly IMapper _mapper;

        public HandleSetGatewayToSite(IRepository<WarehouseSiteEntity> repository, IMapper mapper)
        {
            _repository = repository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(SetGatewayToSite request, CancellationToken cancellationToken)
        {
            var site = await _repository.GetAsync(request.SiteId, cancellationToken);
            var gw = site.Gateways.FirstOrDefault(gw =>
                gw.MacAddress.Equals(request.MacAddress, StringComparison.InvariantCultureIgnoreCase));
            if (gw != null) site.Gateways.Remove(gw);
            site.Gateways.Add(_mapper.Map<Gateway>(request));
            await _repository.UpdateAsync(site, cancellationToken);

            return Unit.Value;
        }
    }
}
