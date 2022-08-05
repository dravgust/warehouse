using System.Text.Json.Serialization;
using Vayosoft.AutoMapper;
using Vayosoft.Core.SharedKernel.Entities;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Entities.Enums;

namespace Warehouse.Core.Entities.Models
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
        public UserType Kind { get; set; }
        public DateTime? Registered { get; set; }
        public DateTime? Deregistered { get; set; }
        public string CultureId { get; set; } = null!;
        public long ProviderId { get; set; }
        object IProvider.ProviderId => ProviderId;
        public LogEventType? LogLevel { get; set; }
        public virtual List<RefreshToken> RefreshTokens { get; } = new();
    }

    [ConventionalMap(typeof(UserEntity), direction: MapDirection.EntityToDto)]
    public class UserEntityDto : IEntity<long>
    {
        object IEntity.Id => Id;
        public long Id { get; set; }
        public string Username { get; set; }
        public string Phone { get; set; }
        public string Email { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserType Kind { get; set; }
        public DateTime? Registered { get; set; }
        public DateTime? Deregistered { get; set; }
        public string CultureId { get; set; }
        public long ProviderId { get; set; }
        public LogEventType? LogLevel { get; set; }

    }
}
