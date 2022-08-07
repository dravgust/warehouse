using MediatR;
using MongoDB.Driver;
using Vayosoft.Core.Commands;
using Vayosoft.Core.Persistence;
using Vayosoft.Core.SharedKernel;
using Vayosoft.Data.MongoDB;
using Warehouse.Core.Entities.Enums;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Entities.ValueObjects;
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
        private readonly UserContext _context;
        private readonly IReadOnlyRepository<BeaconRegisteredEntity> _beaconRegisteredReadOnly;

        public HandleSetBeacon(
            IRepository<BeaconEntity> beaconRepository,
            IRepository<BeaconRegisteredEntity> beaconRegisteredRepository,
            IMapper mapper, UserContext context, IReadOnlyRepository<BeaconRegisteredEntity> beaconRegisteredReadOnly)
        {
            _beaconRepository = beaconRepository;
            _beaconRegisteredRepository = beaconRegisteredRepository;
            _mapper = mapper;
            _context = context;
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
                entity.ProviderId = _context.ProviderId ?? 0;
                await _beaconRepository.AddAsync(entity, cancellationToken);
            }

            return Unit.Value;
        }
    }
}
