using FluentValidation;
using MediatR;
using Vayosoft.Core.Commands;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.SharedKernel.ValueObjects;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Entities.Enums;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Persistence;
using Warehouse.Core.Services;
using Warehouse.Core.Services.Security;
using Warehouse.Core.UseCases.Management.Models;

namespace Warehouse.Core.UseCases.Management.Commands
{
    public sealed class SetBeacon : ProductItemDto, ICommand
    {
        public class SetBeaconValidator : AbstractValidator<SetBeacon>
        {
            public SetBeaconValidator()
            {
                RuleFor(c => c.MacAddress)
                    .NotEmpty()
                    .MacAddress();
            }
        }
    }

    internal sealed class HandleSetBeacon : ICommandHandler<SetBeacon>
    {
        private readonly WarehouseStore _store;
        private readonly IUserContext _userContext;
        private readonly IMapper _mapper;

        public HandleSetBeacon(WarehouseStore store, IMapper mapper, IUserContext userContext)
        {
            _mapper = mapper;
            _userContext = userContext;
            _store = store;
        }

        public async Task<Unit> Handle(SetBeacon request, CancellationToken cancellationToken)
        {
            request.MacAddress = request.MacAddress.ToUpper();
            var providerId = _userContext.User.Identity.GetProviderId();

            if (await _store.BeaconRegistered.FindAsync(request.MacAddress, cancellationToken) == null)
            {
                var rb = new BeaconRegisteredEntity
                {
                    MacAddress = request.MacAddress,
                    ReceivedAt = DateTime.UtcNow,
                    BeaconType = BeaconType.Registered,
                    ProviderId = providerId
                };
                await _store.BeaconRegistered.AddAsync(rb, cancellationToken: cancellationToken);
            }

            if (await _store.TrackedItems.FindAsync(request.MacAddress, cancellationToken) == null)
            {
                var registerTrackedItemResult = TrackedItem.Create(request.MacAddress, providerId);
                if (registerTrackedItemResult.IsError)
                {

                }

                await _store.TrackedItems.AddAsync(registerTrackedItemResult.Value, cancellationToken);
            }
            
            BeaconEntity entity;
            if ((entity = await _store.Beacons.FindAsync(request.MacAddress, cancellationToken)) != null)
            {
                entity.Name = request.Name;
                entity.ProductId = request.Product?.Id;
                entity.Metadata = request.Metadata;
                await _store.Beacons.UpdateAsync(entity, cancellationToken);
            }
            else
            {
                entity = _mapper.Map<BeaconEntity>(request);
                entity.ProviderId = providerId;
                await _store.Beacons.AddAsync(entity, cancellationToken);
            }

            return Unit.Value;
        }
    }
}
