using FluentValidation;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Utilities;
using Warehouse.Core.UseCases.Warehouse.Models;

namespace Warehouse.Core.UseCases.Warehouse.Commands
{
    public class SetGatewayToSite : GatewayDto, ICommand
    {
        public string SiteId { set; get; }
        public class WarehouseRequestValidator : AbstractValidator<SetGatewayToSite>
        {
            public WarehouseRequestValidator()
            {
                RuleFor(q => q.Name).NotEmpty();
                RuleFor(q => q.SiteId).NotEmpty();
                RuleFor(q => q.MacAddress).MacAddress();
            }
        }
    }
}
