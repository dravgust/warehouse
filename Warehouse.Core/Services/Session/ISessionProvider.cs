namespace Warehouse.Core.Services.Session
{
    public interface ISessionProvider
    {
        public long UserId { get; set; }
        public long ProviderId { get; set; }
    }
}
