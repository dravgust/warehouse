using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vayosoft.Data.EF.MySQL;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.Infrastructure.Persistence.Mapping
{
    public class ProviderEntityMap : EntityConfigurationMapper<ProviderEntity>
    {
        public override void Configure(EntityTypeBuilder<ProviderEntity> builder)
        {
            builder.ToTable("providers").HasKey(t => t.Id);
            builder.Property(t => t.Id).HasColumnName("providerid").UseMySqlIdentityColumn();
            builder.Property(t => t.Name).HasColumnName("provider_name").IsRequired();
            builder.Property(t => t.Description).HasColumnName("provider_desc");
            builder.Property(t => t.Flags).HasColumnName("flags").IsRequired().HasDefaultValue(0);
            builder.Property(t => t.Parent).HasColumnName("parent_providerid");
            builder.Property(t => t.Culture).HasColumnName("cultureid");
            builder.Property(t => t.Alias).HasColumnName("alias").IsRequired();
        }
    }
}
