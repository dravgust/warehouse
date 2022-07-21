namespace Warehouse.Core.UseCases.IPS.Models
{
    public class IndoorPositionStatusDto
    {
        public SiteInfo Site { set; get; }
        public ICollection<string> In { set; get; }
        public ICollection<string> Out { set; get; }
    }
}
