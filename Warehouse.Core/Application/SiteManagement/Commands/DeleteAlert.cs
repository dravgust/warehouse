using FluentValidation;
using MediatR;
using Vayosoft.Commands;
using Vayosoft.Persistence;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.Application.SiteManagement.Commands
{
    public sealed class DeleteAlert : ICommand
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

    internal sealed class HandleDeleteAlert : ICommandHandler<DeleteAlert>
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
