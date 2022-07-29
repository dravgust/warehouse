using Vayosoft.Core.SharedKernel.Events;
using Warehouse.Core.Entities.Models;

namespace Warehouse.Core.UseCases.Tracking.Events
{
    public record UserEventOccurred(BeaconReceivedEntity Beacon) : IEvent
    { }
}
