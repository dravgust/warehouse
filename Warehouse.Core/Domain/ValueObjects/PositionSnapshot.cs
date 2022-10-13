namespace Warehouse.Core.Domain.ValueObjects
{
    public record PositionSnapshot
    {
        public DateTime TimeStamp { set; get; }
        public HashSet<string> In { set; get; }
        public HashSet<string> Out { set; get; }
    }
}
