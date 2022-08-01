using FluentValidation;
using Vayosoft.Core.Commands;
using Warehouse.Core.Entities.Models;

namespace Warehouse.Core.UseCases.Management.Commands
{
    public class DeleteAlert : ICommand
    {
        public string Id { get; set; }
        public class AlertRequestValidator : AbstractValidator<AlertEntity>
        {
            public AlertRequestValidator()
            {
                RuleFor(q => q.Id).NotEmpty();
            }
        }
    }
}
