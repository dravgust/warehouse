using Vayosoft.Core.SharedKernel.Events;
using Warehouse.Core.Entities.Models;

namespace Warehouse.Core.UseCases.BeaconTracking.Events
{
    public record UserEventOccurred(BeaconReceivedEntity Beacon) : IEvent
    { }
}
