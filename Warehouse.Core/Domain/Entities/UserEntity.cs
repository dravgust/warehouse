using Vayosoft.Commons.Entities;
using Vayosoft.Utilities;
using Warehouse.Core.Domain.Entities.Identity;
using Warehouse.Core.Domain.Enums;
using Warehouse.Core.Domain.ValueObjects;

namespace Warehouse.Core.Domain.Entities
{
    public class UserEntity : EntityBase<long>, IUser, IProvider<long>
    {
        private UserEntity() { }
        public UserEntity(string username)
        {
            Username = Guard.NotEmpty(username, nameof(username));
        }

        public string Username { get; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Phone { get; set; } = null!;
        public string Email { get; set; } = null!;
        public UserType Type { get; set; }
        public DateTime? Registered { get; set; }
        public DateTime? Deregistered { get; set; }
        public string CultureId { get; set; } = null!;
        public long ProviderId { get; set; }
        object IProvider.ProviderId => ProviderId;
        public LogEventType? LogLevel { get; set; }
        public virtual List<RefreshToken> RefreshTokens { get; } = new();
    }
}
