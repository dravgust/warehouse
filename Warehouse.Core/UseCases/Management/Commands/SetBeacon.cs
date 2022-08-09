using MediatR;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel;
using Warehouse.Core.Entities.Enums;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services.Session;
using Warehouse.Core.UseCases.Management.Models;
using Warehouse.Core.Utilities;

namespace Warehouse.Core.UseCases.Management.Commands
{
    public class SetBeacon : ProductItemDto, ICommand
    { }

    internal class HandleSetBeacon : ICommandHandler<SetBeacon>
    {
        private readonly IRepository<BeaconEntity> _beaconRepository;
        private readonly IRepository<BeaconRegisteredEntity> _beaconRegisteredRepository;
        private readonly IMapper _mapper;
        private readonly ISessionProvider _session;
        private readonly IReadOnlyRepository<BeaconRegisteredEntity> _beaconRegisteredReadOnly;

        public HandleSetBeacon(
            IRepository<BeaconEntity> beaconRepository,
            IRepository<BeaconRegisteredEntity> beaconRegisteredRepository,
            IMapper mapper, ISessionProvider session, IReadOnlyRepository<BeaconRegisteredEntity> beaconRegisteredReadOnly)
        {
            _beaconRepository = beaconRepository;
            _beaconRegisteredRepository = beaconRegisteredRepository;
            _mapper = mapper;
            _session = session;
            _beaconRegisteredReadOnly = beaconRegisteredReadOnly;
        }

        public async Task<Unit> Handle(SetBeacon request, CancellationToken cancellationToken)
        {
            var b = await _beaconRegisteredReadOnly
                .FirstOrDefaultAsync(r => r.MacAddress == request.MacAddress, cancellationToken);
            //var b = await _beaconRegisteredRepository.FindAsync(request.MacAddress, cancellationToken);
            if (b == null)
            {
                var rb = new BeaconRegisteredEntity
                {
                    MacAddress = request.MacAddress,
                    ReceivedAt = DateTime.UtcNow,
                    BeaconType = BeaconType.Registered
                };
                await _beaconRegisteredRepository.AddAsync(rb, cancellationToken: cancellationToken);
            }

            BeaconEntity entity;
            if (!string.IsNullOrEmpty(request.MacAddress) && (entity = await _beaconRepository.FindAsync(request.MacAddress, cancellationToken)) != null)
            {
                entity.Name = request.Name;
                entity.ProductId = request.Product?.Id;
                entity.Metadata = request.Metadata;
                await _beaconRepository.UpdateAsync(entity, cancellationToken);
            }
            else
            {
                entity = _mapper.Map<BeaconEntity>(request);
                var providerId = _session.User.Identity.GetProviderId();
                entity.ProviderId = providerId;
                await _beaconRepository.AddAsync(entity, cancellationToken);
            }

            return Unit.Value;
        }
    }
}
