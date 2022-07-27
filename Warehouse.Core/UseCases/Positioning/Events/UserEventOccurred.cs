using Vayosoft.Core.SharedKernel.Events;
using Warehouse.Core.Entities.Models;

namespace Warehouse.Core.UseCases.Positioning.Events
{
    public record UserEventOccurred(BeaconReceivedEntity Beacon) : IEvent
    { }
}
