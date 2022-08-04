using FluentValidation;
using MediatR;
using Vayosoft.Core.Commands;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Persistence;

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
        private readonly WarehouseStore _store;

        public HandleDeleteBeacon(WarehouseStore store)
        {
            _store = store;
        }

        public async Task<Unit> Handle(DeleteBeacon request, CancellationToken cancellationToken)
        {
            await _store.DeleteAsync(new BeaconEntity { Id = request.MacAddress }, cancellationToken);

            return Unit.Value;
        }
    }
}
