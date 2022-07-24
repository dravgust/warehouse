using FluentValidation;
using Vayosoft.Core.Commands;

namespace Warehouse.Core.UseCases.Management.Commands
{
    public class DeleteProduct : ICommand
    {
        public string Id { get; set; } = null!;

        public class CertificateRequestValidator : AbstractValidator<DeleteProduct>
        {
            public CertificateRequestValidator()
            {
                RuleFor(q => q.Id).NotEmpty();
            }
        }
    }
}
