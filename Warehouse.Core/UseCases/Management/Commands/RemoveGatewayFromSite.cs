using FluentValidation;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Utilities;

namespace Warehouse.Core.UseCases.Management.Commands
{
    public class RemoveGatewayFromSite : ICommand
    {
        public string SiteId { set; get; }
        public string MacAddress { set; get; }
        public class WarehouseRequestValidator : AbstractValidator<RemoveGatewayFromSite>
        {
            public WarehouseRequestValidator()
            {
                RuleFor(q => q.SiteId).NotEmpty();
                RuleFor(q => q.MacAddress).MacAddress();
            }
        }
    }
}
