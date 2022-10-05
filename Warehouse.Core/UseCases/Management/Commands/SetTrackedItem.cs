using FluentValidation;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Persistence;
using Warehouse.Core.Services;
using Warehouse.Core.Services.Security;
using Warehouse.Core.UseCases.Management.Models;
using LanguageExt.Common;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.UseCases.Management.Commands
{
    public sealed class SetTrackedItem : TrackedItemDto, ICommand<Result<TrackedItem>>
    {
        public class SetTrackedItemValidator : AbstractValidator<SetTrackedItem>
        {
            public SetTrackedItemValidator()
            {
                //RuleLevelCascadeMode = CascadeMode.Stop;

                RuleFor(c => c.MacAddress)
                    .NotEmpty()
                    .MacAddress();
            }
        }
    }

    internal sealed class HandleSetBeacon : ICommandHandler<SetTrackedItem, Result<TrackedItem>>
    {
        private readonly WarehouseStore _store;
        private readonly IUserContext _userContext;
        private readonly IValidator<SetTrackedItem> _validator;

        public HandleSetBeacon(WarehouseStore store, IUserContext userContext, IValidator<SetTrackedItem> validator)
        {
            _userContext = userContext;
            _validator = validator;
            _store = store;
        }

        public async Task<Result<TrackedItem>> Handle(SetTrackedItem request, CancellationToken cancellationToken)
        {
            var validationResult = await _validator.ValidateAsync(request, cancellationToken);
            if (!validationResult.IsValid)
            {
                var exception = new ValidationException(validationResult.Errors);
                return new Result<TrackedItem>(exception);
                //return validationResult.Errors.ToErrorOr<TrackedItem>();
            }

            request.MacAddress = request.MacAddress.ToUpper();
            var providerId = _userContext.User.Identity.GetProviderId();

            TrackedItem trackedItem;
            if ((trackedItem = await _store.TrackedItems.FindAsync(request.MacAddress, cancellationToken)) == null)
            {
                var registerTrackedItemResult = TrackedItem.Create(request.MacAddress, providerId);
                trackedItem = registerTrackedItemResult.Value;
                await _store.TrackedItems.AddAsync(trackedItem, cancellationToken);
            }

            if (request.Name != null)
            {
                trackedItem.Name = request.Name;
            }
            if (request.Product != null)
            {
                trackedItem.ProductId = request.Product.Id;
            }
            if (request.Metadata != null)
            {
                trackedItem.Metadata = request.Metadata;
            }
            await _store.TrackedItems.UpdateAsync(trackedItem, cancellationToken);

            return trackedItem;
        }
    }
}
