namespace Warehouse.Core.Application.PositioningSystem.Domain
{
    public class IndoorPositionStatus
    {
        public HashSet<string> In { set; get; }
        public HashSet<string> Out { set; get; }
    }
}
