using Vayosoft.AutoMapper;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.WebAPI.Entities;

namespace Warehouse.Core.Domain.Entities
{
    public class UserEntity : EntityBase<long>, IIdentityUser
    {
        public string? Username { get; set; }
        public string? Email { get; set; }
        public string? PasswordHash { get; set; }
        public virtual List<RefreshToken> RefreshTokens { get; set; } = new();
    }

    [ConventionalMap(typeof(UserEntity))]
    public class UserEntityDto : IEntity<long>
    {
        public long Id { get; set; }
        public string? Username { get; set; }
        public string? Email { get; set; }
        object IEntity.Id => Id;

    }
}
