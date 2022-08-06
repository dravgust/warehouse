using FluentValidation;
using MediatR;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel.Events;
using Warehouse.Core.Entities.Enums;
using Warehouse.Core.Entities.Events;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.UseCases.Administration.Models;

namespace Warehouse.Core.UseCases.Management.Commands
{
    public class DeleteWarehouseSite : ICommand
    {
        public string Id { get; set; }
        public class WarehouseRequestValidator : AbstractValidator<DeleteWarehouseSite>
        {
            public WarehouseRequestValidator()
            {
                RuleFor(q => q.Id).NotEmpty();
            }
        }
    }

    internal class HandleDeleteWarehouseSite : ICommandHandler<DeleteWarehouseSite>
    {
        private readonly IRepository<WarehouseSiteEntity> _repository;
        private readonly IEventBus _eventBus;

        public HandleDeleteWarehouseSite(IRepository<WarehouseSiteEntity> repository, IEventBus eventBus)
        {
            _repository = repository;
            _eventBus = eventBus;
        }

        public async Task<Unit> Handle(DeleteWarehouseSite request, CancellationToken cancellationToken)
        {
            await _repository.DeleteAsync(new WarehouseSiteEntity { Id = request.Id }, cancellationToken);

            var events = new IEvent[]
            {
                OperationEvent.Create(nameof(HandleSetAlert), OperationType.Delete, DateTime.UtcNow, Provider.Default.ToString())
            };
            await _eventBus.Publish(events);
            return Unit.Value;
        }
    }
}
