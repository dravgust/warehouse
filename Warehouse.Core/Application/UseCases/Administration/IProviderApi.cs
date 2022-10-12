namespace Warehouse.Core.Application.UseCases.Administration
{
    public interface IProviderApi
    {
        string GetMsisdnByImei(string imei);

        bool IsUserActive(string msisdn);
    }
}
