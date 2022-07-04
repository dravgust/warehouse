using MediatR;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.Events;
using Warehouse.Core.Entities.Enums;
using Warehouse.Core.Entities.Events;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.Products.Commands;
using Warehouse.Core.UseCases.Providers.Models;

namespace Warehouse.Core.UseCases.Products.Handlers
{
    public class ProductCommandHandler :
        ICommandHandler<SetProduct>,
        ICommandHandler<DeleteProduct>
    {
        private readonly IRepository<ProductEntity, string> _repository;
        private readonly IEventBus _eventBus;
        private readonly IMapper _mapper;

        public ProductCommandHandler(
            IRepository<ProductEntity, string> repository,
            IEventBus eventBus,
            IMapper mapper)
        {
            _repository = repository;
            _eventBus = eventBus;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(SetProduct request, CancellationToken cancellationToken)
        {
            ProductEntity? entity;
            if (!string.IsNullOrEmpty(request.Id) && (entity = await _repository.FindAsync(request.Id, cancellationToken)) != null)
                await _repository.UpdateAsync(_mapper.Map(request, entity), cancellationToken);
            else
            {
                await _repository.AddAsync(_mapper.Map<ProductEntity>(request), cancellationToken);
            }

            return Unit.Value;
        }

        public async Task<Unit> Handle(DeleteProduct request, CancellationToken cancellationToken)
        {
            await _repository.DeleteAsync(new ProductEntity { Id = request.Id }, cancellationToken);

            var events = new IEvent[]
            {
                OperationOccurred.Create(nameof(ProductCommandHandler), OperationType.Delete, DateTime.UtcNow, Provider.Default.ToString())
            };
            await _eventBus.Publish(events);
            return Unit.Value;
        }
    }
}
