using FluentValidation;
using Vayosoft.Core.Commands;

namespace Warehouse.Core.UseCases.Products.Commands
{
    public class DeleteProduct : ICommand
    {
        public string Id { get; set; }
        public class CertificateRequestValidator : AbstractValidator<DeleteProduct>
        {
            public CertificateRequestValidator()
            {
                RuleFor(q => q.Id).NotEmpty();
            }
        }
    }
}
