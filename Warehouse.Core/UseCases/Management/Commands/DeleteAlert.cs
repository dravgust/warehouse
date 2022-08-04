using FluentValidation;
using MediatR;
using Vayosoft.Core.Commands;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Persistence;

namespace Warehouse.Core.UseCases.Management.Commands
{
    public class DeleteAlert : ICommand
    {
        public string Id { get; set; }
        public class AlertRequestValidator : AbstractValidator<AlertEntity>
        {
            public AlertRequestValidator()
            {
                RuleFor(q => q.Id).NotEmpty();
            }
        }
    }

    internal class HandleDeleteAlert : ICommandHandler<DeleteAlert>
    {
        private readonly WarehouseStore _store;

        public HandleDeleteAlert(WarehouseStore store)
        {
            _store = store;
        }

        public async Task<Unit> Handle(DeleteAlert request, CancellationToken cancellationToken)
        {
            //todo delete notification on delete alert event
            await _store.DeleteAsync<NotificationEntity>(e => e.AlertId == request.Id, cancellationToken);
            await _store.DeleteAsync(new AlertEntity { Id = request.Id }, cancellationToken);
            return Unit.Value;
        }
    }
}
