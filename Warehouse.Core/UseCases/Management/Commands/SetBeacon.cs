using MediatR;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel;
using Warehouse.Core.Entities.Enums;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Persistence;
using Warehouse.Core.UseCases.Management.Models;

namespace Warehouse.Core.UseCases.Management.Commands
{
    public class SetBeacon : ProductItemDto, ICommand
    { }

    internal class HandleSetBeacon : ICommandHandler<SetBeacon>
    {
        private readonly IRepository<BeaconEntity> _beaconRepository;
        private readonly IRepository<BeaconRegisteredEntity> _beaconRegisteredRepository;
        private readonly IMapper _mapper;

        public HandleSetBeacon(
            IRepository<BeaconEntity> beaconRepository,
            IRepository<BeaconRegisteredEntity> beaconRegisteredRepository,
            IMapper mapper)
        {
            _beaconRepository = beaconRepository;
            _beaconRegisteredRepository = beaconRegisteredRepository;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(SetBeacon request, CancellationToken cancellationToken)
        {
            var b = await _beaconRegisteredRepository.FindAsync(request.MacAddress, cancellationToken);
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
                await _beaconRepository.UpdateAsync(_mapper.Map(request, entity), cancellationToken);
            }
            else
            {
                await _beaconRepository.AddAsync(_mapper.Map<BeaconEntity>(request), cancellationToken);
            }

            return Unit.Value;
        }
    }
}
