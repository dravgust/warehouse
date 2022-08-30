using FluentValidation;
using MediatR;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Persistence;
using Warehouse.Core.Entities.Models;

namespace Warehouse.Core.UseCases.Management.Commands
{
    public class DeleteAlert : ICommand
    {
        public string Id { get; set; }
        public class AlertRequestValidator : AbstractValidator<DeleteAlert>
        {
            public AlertRequestValidator()
            {
                RuleFor(q => q.Id).NotEmpty();
            }
        }
    }

    internal class HandleDeleteAlert : ICommandHandler<DeleteAlert>
    {
        private readonly IRepository<AlertEntity> _repository;

        public HandleDeleteAlert(IRepository<AlertEntity> repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteAlert request, CancellationToken cancellationToken)
        {
            //todo delete notification on delete alert event
            await _repository.DeleteAsync(new AlertEntity { Id = request.Id }, cancellationToken);
            return Unit.Value;
        }
    }
}
