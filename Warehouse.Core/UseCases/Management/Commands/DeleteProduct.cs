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
    public class DeleteProduct : ICommand
    {
        public string Id { get; set; } = null!;

        public class CertificateRequestValidator : AbstractValidator<DeleteProduct>
        {
            public CertificateRequestValidator()
            {
                RuleFor(q => q.Id).NotEmpty();
            }
        }
    }

    public class HandleDeleteProduct :
        ICommandHandler<DeleteProduct>
    {
        private readonly IRepository<ProductEntity> _repository;
        private readonly IEventBus _eventBus;

        public HandleDeleteProduct(
            IRepository<ProductEntity> repository,
            IEventBus eventBus)
        {
            _repository = repository;
            _eventBus = eventBus;
        }

        public async Task<Unit> Handle(DeleteProduct request, CancellationToken cancellationToken)
        {
            await _repository.DeleteAsync(new ProductEntity { Id = request.Id }, cancellationToken);

            var events = new IEvent[]
            {
                OperationEvent.Create(nameof(HandleDeleteProduct), OperationType.Delete, DateTime.UtcNow, Provider.Default.ToString())
            };
            await _eventBus.Publish(events);
            return Unit.Value;
        }
    }
}
