using MediatR;
using Vayosoft.Core.Commands;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Events;
using Warehouse.Core.Entities.Enums;
using Warehouse.Core.Entities.Events;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Persistence;
using Warehouse.Core.UseCases.Administration.Models;

namespace Warehouse.Core.UseCases.Management.Commands
{
    public class WarehouseCommandHandler :
        ICommandHandler<SetWarehouseSite>,
        ICommandHandler<DeleteWarehouseSite>,
        ICommandHandler<SetGatewayToSite>,
        ICommandHandler<RemoveGatewayFromSite>,
        ICommandHandler<SetBeacon>,
        ICommandHandler<DeleteBeacon>
    {
        private readonly WarehouseStore _store;
        private readonly IEventBus _eventBus;
        private readonly IMapper _mapper;

        public WarehouseCommandHandler(WarehouseStore store, IEventBus eventBus, IMapper mapper)
        {
            _store = store;
            _eventBus = eventBus;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(SetWarehouseSite request, CancellationToken cancellationToken)
        {
            await _store.SetAsync(_mapper.Map<WarehouseSiteEntity>(request), cancellationToken);
            return Unit.Value;
        }

        public async Task<Unit> Handle(DeleteWarehouseSite request, CancellationToken cancellationToken)
        {
            await _store.DeleteAsync(new WarehouseSiteEntity { Id = request.Id }, cancellationToken);

            var events = new IEvent[]
            {
                OperationEvent.Create(nameof(WarehouseCommandHandler), OperationType.Delete, DateTime.UtcNow, Provider.Default.ToString())
            };
            await _eventBus.Publish(events);
            return Unit.Value;
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

        public async Task<Unit> Handle(RemoveGatewayFromSite request, CancellationToken cancellationToken)
        {
            var site = await _store.GetAsync<WarehouseSiteEntity>(request.SiteId, cancellationToken);
            var gw = site.Gateways.FirstOrDefault(gw =>
                gw.MacAddress.Equals(request.MacAddress, StringComparison.InvariantCultureIgnoreCase));
            if (gw != null) site.Gateways.Remove(gw);
            await _store.UpdateAsync(site, cancellationToken);

            return Unit.Value;
        }

        public async Task<Unit> Handle(SetBeacon request, CancellationToken cancellationToken)
        {
            var b = await _store.FirstOrDefaultAsync<BeaconRegisteredEntity>(r => r.MacAddress == request.MacAddress, cancellationToken);
            if (b == null)
            {
                var rb = new BeaconRegisteredEntity
                {
                    MacAddress = request.MacAddress,
                    ReceivedAt = DateTime.UtcNow,
                    BeaconType = BeaconType.Registered
                };
                await _store.AddAsync(rb, cancellationToken: cancellationToken);
            }

            BeaconEntity entity;
            if (!string.IsNullOrEmpty(request.MacAddress) && (entity = await _store.FindAsync<BeaconEntity>(request.MacAddress, cancellationToken)) != null)
            {
                await _store.UpdateAsync(_mapper.Map(request, entity), cancellationToken);
            }
            else
            {
                await _store.AddAsync(_mapper.Map<BeaconEntity>(request), cancellationToken);
            }

            return Unit.Value;
        }

        public async Task<Unit> Handle(DeleteBeacon request, CancellationToken cancellationToken)
        {
            await _store.DeleteAsync(new BeaconEntity { Id = request.MacAddress }, cancellationToken);

            return Unit.Value;
        }
    }

}
