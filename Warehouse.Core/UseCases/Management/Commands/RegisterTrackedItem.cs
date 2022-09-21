using Vayosoft.Core.Commands;
using ErrorOr;
using FluentValidation;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services;
using Warehouse.Core.Services.Security;

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
    private readonly IUserContext _userContext;

    public HandleRegisterTrackedItem(IRepositoryBase<TrackedItem> trackedItemsRepo, IUserContext userContext)
    {
        _trackedItemsRepo = trackedItemsRepo;
        _userContext = userContext;
    }

    public async Task<ErrorOr<TrackedItem>> Handle(RegisterTrackedItem request, CancellationToken cancellationToken)
    {
        var providerId = _userContext.User.Identity.GetProviderId();
        var registerTrackedItemResult = TrackedItem.Create(request.MacAddress, providerId);
        if (registerTrackedItemResult.IsError)
        {
            return registerTrackedItemResult.Errors;
        }

        await _trackedItemsRepo.AddAsync(registerTrackedItemResult.Value, cancellationToken);
        return registerTrackedItemResult.Value;
    }
}
