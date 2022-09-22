using FluentValidation;
using MediatR;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Persistence;
using Warehouse.Core.Services;
using Warehouse.Core.Services.Security;
using Warehouse.Core.UseCases.Management.Models;

namespace Warehouse.Core.UseCases.Management.Commands
{
    public sealed class SetTrackedItem : TrackedItemDto, ICommand
    {
        public class SetBeaconValidator : AbstractValidator<SetTrackedItem>
        {
            public SetBeaconValidator()
            {
                RuleFor(c => c.MacAddress)
                    .NotEmpty()
                    .MacAddress();
            }
        }
    }

    internal sealed class HandleSetBeacon : ICommandHandler<SetTrackedItem>
    {
        private readonly WarehouseStore _store;
        private readonly IUserContext _userContext;

        public HandleSetBeacon(WarehouseStore store, IUserContext userContext)
        {
            _userContext = userContext;
            _store = store;
        }

        public async Task<Unit> Handle(SetTrackedItem request, CancellationToken cancellationToken)
        {
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

            return Unit.Value;
        }
    }
}
