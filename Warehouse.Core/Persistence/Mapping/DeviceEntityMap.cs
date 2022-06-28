using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vayosoft.Data.EF.MySQL;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.Persistence.Mapping
{
    public partial class DeviceEntityMap : EntityConfigurationMapper<DeviceEntity>
    {
        public override void Configure(EntityTypeBuilder<DeviceEntity> builder)
        {
            builder.ToTable("devices").HasKey(t => t.Id);
            builder.Property(t => t.Id).HasColumnName("deviceid").ValueGeneratedOnAdd();
            builder.Property(t => t.MacAddress).HasColumnName("mac_address");
            builder.Property(t => t.ProviderId).HasColumnName("providerid");

        }
    }
}
