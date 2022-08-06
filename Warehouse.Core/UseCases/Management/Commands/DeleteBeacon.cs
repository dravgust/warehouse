using FluentValidation;
using MediatR;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Persistence;
using Warehouse.Core.Entities.Models;

namespace Warehouse.Core.UseCases.Management.Commands
{
    public class DeleteBeacon : ICommand
    {
        public string MacAddress { get; set; }
        public class WarehouseRequestValidator : AbstractValidator<DeleteBeacon>
        {
            public WarehouseRequestValidator()
            {
                RuleFor(q => q.MacAddress).NotEmpty();
            }
        }
    }

    internal class HandleDeleteBeacon : ICommandHandler<DeleteBeacon>
    {
        private readonly IRepository<BeaconEntity> _repository;

        public HandleDeleteBeacon(IRepository<BeaconEntity> repository)
        {
            _repository = repository;
        }

        public async Task<Unit> Handle(DeleteBeacon request, CancellationToken cancellationToken)
        {
            await _repository.DeleteAsync(new BeaconEntity { Id = request.MacAddress }, cancellationToken);

            return Unit.Value;
        }
    }
}
