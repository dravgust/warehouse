namespace Warehouse.Core.UseCases.Providers
{
    public interface IProviderApi
    {
        string GetMsisdnByImei(string imei);

        bool IsUserActive(string msisdn);
    }
}
