namespace Warehouse.Core.Application.SystemAdministration
{
    public interface IProviderApi
    {
        string GetMsisdnByImei(string imei);

        bool IsUserActive(string msisdn);
    }
}
