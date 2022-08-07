namespace Warehouse.Core.Entities.Enums
{
    public enum UserType
    {
        DeviceUser = 0,
        Support = 1,
        Developer = 2,
        Administrator = 3,
        Supervisor = 4,

        TechnicalUser = -1,
        HealthChecker = -10,
        Guest = -100
    }
}
