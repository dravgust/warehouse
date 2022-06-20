using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vayosoft.Data.EF.MySQL;
using Warehouse.Core.Domain.Entities;

namespace Warehouse.Core.Persistence.Mapping
{
    public partial class UserEntityMap : EntityConfigurationMapper<UserEntity>
    {
        public override void Configure(EntityTypeBuilder<UserEntity> builder)
        {
            builder.ToTable("users").HasKey(t => t.Id);
            builder.Property(t => t.Id).HasColumnName("userid").ValueGeneratedOnAdd();
            builder.Property(t => t.Username).HasColumnName("username");
            builder.Property(t => t.Email).HasColumnName("email");
            builder.Property(t => t.Password).HasColumnName("pwdhash");
        }
    }
}
