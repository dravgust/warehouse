using FluentValidation;
using MediatR;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Persistence;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.Application.SiteManagement.Commands
{
    public sealed class DeleteTrackedItem : ICommand
    {
        public string MacAddress { get; set; }
        public class WarehouseRequestValidator : AbstractValidator<DeleteTrackedItem>
        {
            public WarehouseRequestValidator()
            {
                RuleFor(q => q.MacAddress).NotEmpty();
            }
        }
    }

    internal sealed class HandleDeleteTrackedItem : ICommandHandler<DeleteTrackedItem>
    {
        private readonly IRepositoryBase<TrackedItem> _repository;

        public HandleDeleteTrackedItem(IRepositoryBase<TrackedItem> repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteTrackedItem request, CancellationToken cancellationToken)
        {
            await _repository.DeleteAsync(request.MacAddress, cancellationToken);

            return Unit.Value;
        }
    }
}
