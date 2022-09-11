using Vayosoft.Core.Commands;
using ErrorOr;
using FluentValidation;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Entities.Models;

namespace Warehouse.Core.UseCases.Management.Commands;

public class RegisterTrackedItem : ICommand<ErrorOr<TrackedItem>>
{
    public string MacAddress { get; set; }

    public class RegisterTrackedItemValidator : AbstractValidator<RegisterTrackedItem>
    {
        public RegisterTrackedItemValidator()
        {
            RuleFor(c => c.MacAddress)
                .NotEmpty()
                .MacAddress();
        }
    }
}

public class HandleRegisterTrackedItem : ICommandHandler<RegisterTrackedItem, ErrorOr<TrackedItem>>
{
    private readonly IRepositoryBase<TrackedItem> _trackedItemsRepo;

    public HandleRegisterTrackedItem(IRepositoryBase<TrackedItem> trackedItemsRepo)
    {
        _trackedItemsRepo = trackedItemsRepo;
    }

    public async Task<ErrorOr<TrackedItem>> Handle(RegisterTrackedItem request, CancellationToken cancellationToken)
    {
        var registerTrackedItemResult = TrackedItem.Register(request.MacAddress);
        if (registerTrackedItemResult.IsError)
        {
            return registerTrackedItemResult.Errors;
        }

        await _trackedItemsRepo.AddAsync(registerTrackedItemResult.Value, cancellationToken);
        return registerTrackedItemResult.Value;
    }
}
