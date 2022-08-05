using MediatR;
using Vayosoft.Core.Commands;
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
        private readonly WarehouseDataStore _store;
        private readonly IMapper _mapper;

        public HandleSetBeacon(WarehouseDataStore store, IMapper mapper)
        {
            _store = store;
            _mapper = mapper;
        }

        public async Task<Unit> Handle(SetBeacon request, CancellationToken cancellationToken)
        {
            var b = await _store.FirstOrDefaultAsync<BeaconRegisteredEntity>(r => r.MacAddress == request.MacAddress, cancellationToken);
            if (b == null)
            {
                var rb = new BeaconRegisteredEntity
                {
                    MacAddress = request.MacAddress,
                    ReceivedAt = DateTime.UtcNow,
                    BeaconType = BeaconType.Registered
                };
                await _store.AddAsync(rb, cancellationToken: cancellationToken);
            }

            BeaconEntity entity;
            if (!string.IsNullOrEmpty(request.MacAddress) && (entity = await _store.FindAsync<BeaconEntity>(request.MacAddress, cancellationToken)) != null)
            {
                await _store.UpdateAsync(_mapper.Map(request, entity), cancellationToken);
            }
            else
            {
                await _store.AddAsync(_mapper.Map<BeaconEntity>(request), cancellationToken);
            }

            return Unit.Value;
        }
    }
}
