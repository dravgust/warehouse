using System.Runtime.Serialization;

namespace Warehouse.Core.Entities.Enums
{
    [DataContract]
    public enum UserType
    {
        [EnumMember]
        DeviceUser = 0,
        [EnumMember]
        Support = 1,
        [EnumMember]
        Developer = 2,
        [EnumMember]
        Administrator = 3,
        [EnumMember]
        Supervisor = 4,

        [EnumMember]
        TechnicalUser = -1,


        [EnumMember]
        HealthChecker = -10,
        [EnumMember]
        Guest = -100
    }
}
