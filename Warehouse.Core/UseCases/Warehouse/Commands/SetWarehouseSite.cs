using FluentValidation;
using Vayosoft.Core.Commands;
using Warehouse.Core.UseCases.Warehouse.Models;

namespace Warehouse.Core.UseCases.Warehouse.Commands
{
    public class SetWarehouseSite : WarehouseSiteDto, ICommand
    {
        public class WarehouseRequestValidator : AbstractValidator<SetWarehouseSite>
        {
            public WarehouseRequestValidator()
            {
                RuleFor(q => q.Name).NotEmpty();
                //RuleFor(q => q.MacAddress).MacAddress();
            }
        }
    }
}
