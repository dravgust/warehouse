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
            builder.ToTable("refresh_tokens").HasKey(t => new { t.UserId, t.Token });
            builder.Property(t => t.UserId).HasColumnName("userid").IsRequired();
            builder.Property(t => t.Token).HasColumnName("token").IsRequired();
            builder.Property(t => t.Created).HasColumnName("created");
            builder.Property(t => t.CreatedByIp).HasColumnName("created_by_ip");
            builder.Property(t => t.Revoked).HasColumnName("revoked");
            builder.Property(t => t.RevokedByIp).HasColumnName("revoked_by_ip");
            builder.Property(t => t.ReasonRevoked).HasColumnName("reason_revoked");
            builder.Property(t => t.ReplacedByToken).HasColumnName("replaced_by_token");
            builder.Property(t => t.Expires).HasColumnName("expires");
            builder
                .HasOne(t => t.User as UserEntity)
                .WithMany(t => t.RefreshTokens)
                .HasForeignKey(t => t.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
