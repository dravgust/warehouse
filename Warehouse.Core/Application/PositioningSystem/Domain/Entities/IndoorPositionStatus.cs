namespace Warehouse.Core.Application.PositioningSystem.Domain.Entities
{
    public class IndoorPositionStatus
    {
        public HashSet<string> In { set; get; }
        public HashSet<string> Out { set; get; }
    }
}
