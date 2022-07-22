using MediatR;
using MongoDB.Driver;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Events;
using Vayosoft.Data.MongoDB;
using Warehouse.Core.Entities.Enums;
using Warehouse.Core.Entities.Events;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.Management.Commands;
using Warehouse.Core.UseCases.Providers.Models;

namespace Warehouse.Core.UseCases.Management.Handlers
{
    public class WarehouseCommandHandler :
        ICommandHandler<SetWarehouseSite>,
        ICommandHandler<DeleteWarehouseSite>,
        ICommandHandler<SetGatewayToSite>,
        ICommandHandler<RemoveGatewayFromSite>,
        ICommandHandler<SetBeacon>,
        ICommandHandler<DeleteBeacon>
    {
        private readonly IRepository<WarehouseSiteEntity> _repository;
        private readonly IEventBus _eventBus;
        private readonly IMapper _mapper;
        private readonly IRepository<BeaconEntity> _beaconRepository;
        private readonly IMongoCollection<BeaconRegisteredEntity> _collection;

        public WarehouseCommandHandler(IRepository<WarehouseSiteEntity> repository, IEventBus eventBus, IMapper mapper, IRepository<BeaconEntity> beaconRepository,
            IMongoContext context)
        {
            _repository = repository;
            _eventBus = eventBus;
            _mapper = mapper;
            _beaconRepository = beaconRepository;
            _collection = context.Database.GetCollection<BeaconRegisteredEntity>(CollectionName.For<BeaconRegisteredEntity>());
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
            var b = await _collection.Find(r => r.MacAddress == request.MacAddress).FirstOrDefaultAsync(cancellationToken: cancellationToken); ;
            if (b == null)
            {
                var rb = new BeaconRegisteredEntity
                {
                    MacAddress = request.MacAddress,
                    ReceivedAt = DateTime.UtcNow,
                    BeaconType = BeaconType.Registered
                };
                await _collection.InsertOneAsync(rb, cancellationToken: cancellationToken);
            }
            BeaconEntity? entity;
            if (!string.IsNullOrEmpty(request.MacAddress) && (entity = await _beaconRepository.FindAsync(request.MacAddress, cancellationToken)) != null)
            {
                await _beaconRepository.UpdateAsync(_mapper.Map(request, entity), cancellationToken);
            }
            else
            {
                await _beaconRepository.AddAsync(_mapper.Map<BeaconEntity>(request), cancellationToken);
            }

            return Unit.Value;
        }

        public async Task<Unit> Handle(DeleteBeacon request, CancellationToken cancellationToken)
        {
            await _beaconRepository.DeleteAsync(new BeaconEntity { Id = request.MacAddress }, cancellationToken);

            return Unit.Value;
        }
    }

}
