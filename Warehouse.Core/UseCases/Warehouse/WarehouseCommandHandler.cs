using MediatR;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Events;
using Warehouse.Core.Entities.Enums;
using Warehouse.Core.Entities.Events;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.Providers.Models;
using Warehouse.Core.UseCases.Warehouse.Commands;

namespace Warehouse.Core.UseCases.Warehouse
{
    public class WarehouseCommandHandler :
        ICommandHandler<SetWarehouseSite>,
        ICommandHandler<DeleteWarehouseSite>,
        ICommandHandler<SetGatewayToSite>,
        ICommandHandler<RemoveGatewayFromSite>,
        ICommandHandler<SetBeacon>
    {
        private readonly IRepository<WarehouseSiteEntity, string> _repository;
        private readonly IEventBus _eventBus;
        private readonly IMapper _mapper;
        private readonly IRepository<BeaconEntity, string> _beaconRepository;

        public WarehouseCommandHandler(IRepository<WarehouseSiteEntity, string> repository, IEventBus eventBus, IMapper mapper, IRepository<BeaconEntity, string> beaconRepository)
        {
            _repository = repository;
            _eventBus = eventBus;
            _mapper = mapper;
            _beaconRepository = beaconRepository;
        }

        public async Task<Unit> Handle(SetWarehouseSite request, CancellationToken cancellationToken)
        {
            WarehouseSiteEntity? entity;
            if (!string.IsNullOrEmpty(request.Id) && (entity = await _repository.FindAsync(request.Id, cancellationToken)) != null)
            {
                await _repository.UpdateAsync(_mapper.Map(request, entity), cancellationToken);
            }
            else
            {
                await _repository.AddAsync(_mapper.Map<WarehouseSiteEntity>(request), cancellationToken);
            }

            return Unit.Value;
        }

        public async Task<Unit> Handle(DeleteWarehouseSite request, CancellationToken cancellationToken)
        {
            await _repository.DeleteAsync(new WarehouseSiteEntity { Id = request.Id }, cancellationToken);

            var events = new IEvent[]
            {
                OperationOccurred.Create(nameof(WarehouseCommandHandler), OperationType.Delete, DateTime.UtcNow, Provider.Default.ToString())
            };
            await _eventBus.Publish(events);
            return Unit.Value;
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

        public async Task<Unit> Handle(RemoveGatewayFromSite request, CancellationToken cancellationToken)
        {
            var site = await _repository.GetAsync(request.SiteId, cancellationToken);
            var gw = site.Gateways.FirstOrDefault(gw =>
                gw.MacAddress.Equals(request.MacAddress, StringComparison.InvariantCultureIgnoreCase));
            if (gw != null) site.Gateways.Remove(gw);
            await _repository.UpdateAsync(site, cancellationToken);

            return Unit.Value;
        }

        public async Task<Unit> Handle(SetBeacon request, CancellationToken cancellationToken)
        {
            BeaconEntity? entity;
            if (!string.IsNullOrEmpty(request.MacAddress) && (entity = await _beaconRepository.FindAsync(request.MacAddress, cancellationToken)) != null)
            {
                if (request.Product != null && !string.IsNullOrEmpty(request.Product.Id))
                {
                    entity.ProductId = request.Product.Id;
                }

                if (!string.IsNullOrEmpty(request.Name))
                    entity.Name = request.Name;

                if (request.Metadata != null)
                {
                    entity.Metadata = request.Metadata;
                }

                await _beaconRepository.UpdateAsync(entity, cancellationToken);
            }
            else
            {
                var newEntity = new BeaconEntity
                {
                    Id = request.MacAddress
                };
                if (request.Product != null && !string.IsNullOrEmpty(request.Product.Id))
                {
                    newEntity.ProductId = request.Product.Id;
                }

                if (!string.IsNullOrEmpty(request.Name))
                    newEntity.Name = request.Name;

                if (request.Metadata != null)
                {
                    newEntity.Metadata = request.Metadata;
                }
                await _beaconRepository.AddAsync(newEntity, cancellationToken);
            }

            return Unit.Value;
        }
    }

}
