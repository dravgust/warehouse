using FluentValidation;
using Vayosoft.Core.Commands;

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
}
