using FluentValidation;
using Vayosoft.Core.Commands;
using Warehouse.Core.UseCases.Management.Models;

namespace Warehouse.Core.UseCases.Management.Commands
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
