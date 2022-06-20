using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Vayosoft.Data.EF.MySQL;
using Vayosoft.WebAPI.Entities;
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
            builder.Property(t => t.PasswordHash).HasColumnName("pwdhash");

            builder
                .HasMany(t => t.RefreshTokens)
                .WithOne(t => t.User as UserEntity)
                .HasForeignKey(t => t.UserId);
        }
    }

    public partial class RefreshTokenMap : EntityConfigurationMapper<RefreshToken>
    {
        public override void Configure(EntityTypeBuilder<RefreshToken> builder)
        {
            builder.ToTable("refresh_tokens").HasKey(t => t.UserId);
            builder
                .HasOne(t => t.User as UserEntity)
                .WithMany(t => t.RefreshTokens)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
