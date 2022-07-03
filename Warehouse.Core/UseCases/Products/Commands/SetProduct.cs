using FluentValidation;
using Vayosoft.Core.Commands;
using Warehouse.Core.UseCases.Products.Models;

namespace Warehouse.Core.UseCases.Products.Commands
{
    public class SetProduct : ProductDto, ICommand
    {
        public class ProductRequestValidator : AbstractValidator<SetProduct>
        {
            public ProductRequestValidator()
            {
                RuleFor(q => q.Name).NotEmpty();
                //RuleFor(q => q.MacAddress).MacAddress();
            }
        }
    }
}
