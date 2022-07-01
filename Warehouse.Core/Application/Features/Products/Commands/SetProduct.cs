using FluentValidation;
using Vayosoft.Core.SharedKernel.Commands;
using Warehouse.Core.Application.ViewModels;

namespace Warehouse.Core.Application.Features.Products.Commands
{
    public class SetProduct : ProductDto, ICommand
    {
        public class CertificateRequestValidator : AbstractValidator<SetProduct>
        {
            public CertificateRequestValidator()
            {
                RuleFor(q => q.Name).NotEmpty();
                //RuleFor(q => q.MacAddress).MacAddress();
            }
        }
    }
}
