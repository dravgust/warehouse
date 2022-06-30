using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;
using Vayosoft.Data.EF.MySQL;
using Warehouse.Core.Domain.Entities;
using Warehouse.Core.Domain.ValueObjects;

namespace Warehouse.Core.Persistence.Mapping
{
    public partial class DeviceEntityMap : EntityConfigurationMapper<DeviceEntity>
    {
        public override void Configure(EntityTypeBuilder<DeviceEntity> builder)
        {
            builder.ToTable("devices").HasKey(t => t.Id);
            builder.Property(t => t.Id).HasColumnName("deviceid").ValueGeneratedOnAdd();
            builder.Property(t => t.MacAddress).HasColumnName("mac_address");
                //.HasConversion<MacAddressConverter>();
            builder.Property(t => t.ProviderId).HasColumnName("providerid");

        }
    }

    public class MacAddressConverter : ValueConverter<MacAddress, string>
    {
        public MacAddressConverter()
            : base(                v => v.ToString(),
                v => MacAddress.Create(v))
        {
        }
    }
}
