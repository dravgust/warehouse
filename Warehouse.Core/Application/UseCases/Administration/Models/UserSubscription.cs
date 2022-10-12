namespace Warehouse.Core.Application.UseCases.Administration.Models
{
    public record UserSubscription
    {
        public bool IsSubscribed { set; get; }
        public string ProviderName { set; get; }
        public Exception Error { set; get; }
    }
}
