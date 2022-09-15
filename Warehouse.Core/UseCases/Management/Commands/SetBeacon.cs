using FluentValidation;
using MediatR;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Entities.Enums;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services;
using Warehouse.Core.Services.Security;
using Warehouse.Core.UseCases.Management.Models;

namespace Warehouse.Core.UseCases.Management.Commands
{
    public class SetBeacon : ProductItemDto, ICommand
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

    internal class HandleSetBeacon : ICommandHandler<SetBeacon>
    {
        private readonly IReadOnlyRepository<BeaconRegisteredEntity> _beaconsRegisteredR;
        private readonly IRepositoryBase<BeaconRegisteredEntity> _beaconsRegistered;
        private readonly IRepositoryBase<BeaconEntity> _beacons;
        private readonly IUserContext _userContext;
        private readonly IMapper _mapper;

        public HandleSetBeacon(
            IRepositoryBase<BeaconEntity> beacons,
            IRepositoryBase<BeaconRegisteredEntity> beaconsRegistered,
            IReadOnlyRepository<BeaconRegisteredEntity> beaconsRegisteredR,
            IMapper mapper, IUserContext userContext)
        {
            _beacons = beacons;
            _beaconsRegistered = beaconsRegistered;
            _mapper = mapper;
            _userContext = userContext;
            _beaconsRegisteredR = beaconsRegisteredR;
        }

        public async Task<Unit> Handle(SetBeacon request, CancellationToken cancellationToken)
        {
            var providerId = _userContext.User.Identity.GetProviderId();
            var b = await _beaconsRegisteredR
                .FirstOrDefaultAsync(r => r.MacAddress == request.MacAddress, cancellationToken);
            //var b = await _beaconRegisteredRepository.FindAsync(request.MacAddress, cancellationToken);
            if (b == null)
            {
                var rb = new BeaconRegisteredEntity
                {
                    MacAddress = request.MacAddress,
                    ReceivedAt = DateTime.UtcNow,
                    BeaconType = BeaconType.Registered,
                    ProviderId = providerId
                };
                await _beaconsRegistered.AddAsync(rb, cancellationToken: cancellationToken);
            }

            BeaconEntity entity;
            if (!string.IsNullOrEmpty(request.MacAddress) && (entity = await _beacons.FindAsync(request.MacAddress, cancellationToken)) != null)
            {
                entity.Name = request.Name;
                entity.ProductId = request.Product?.Id;
                entity.Metadata = request.Metadata;
                await _beacons.UpdateAsync(entity, cancellationToken);
            }
            else
            {
                entity = _mapper.Map<BeaconEntity>(request);
                entity.ProviderId = providerId;
                await _beacons.AddAsync(entity, cancellationToken);
            }

            return Unit.Value;
        }
    }
}
