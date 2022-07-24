using FluentValidation;
using Vayosoft.Core.Commands;

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
}
