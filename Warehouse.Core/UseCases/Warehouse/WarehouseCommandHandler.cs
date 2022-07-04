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
    public class WarehouseCommandHandler : ICommandHandler<SetWarehouseSite>, ICommandHandler<DeleteWarehouseSite>, ICommandHandler<SetGatewayToSite>
    {
        private readonly IRepository<WarehouseSiteEntity, string> _repository;
        private readonly IEventBus _eventBus;
        private readonly IMapper _mapper;

        public WarehouseCommandHandler(IRepository<WarehouseSiteEntity, string> repository, IEventBus eventBus, IMapper mapper)
        {
            _repository = repository;
            _eventBus = eventBus;
            _mapper = mapper;
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

        public Task<Unit> Handle(SetGatewayToSite request, CancellationToken cancellationToken)
        {
            throw new NotImplementedException();
        }
    }
}
