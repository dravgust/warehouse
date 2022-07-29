namespace Warehouse.Core.UseCases.Administration
{
    public interface IProviderApi
    {
        string GetMsisdnByImei(string imei);

        bool IsUserActive(string msisdn);
    }
}
