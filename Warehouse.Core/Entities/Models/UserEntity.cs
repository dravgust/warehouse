using System.Text.Json.Serialization;
using Vayosoft.AutoMapper;
using Vayosoft.Core.SharedKernel.Entities;
using Warehouse.Core.Entities.Enums;
using Warehouse.Core.UseCases.Administration.Models;

namespace Warehouse.Core.Entities.Models
{
    public class UserEntity : EntityBase<long>, IIdentityUser
    {
        public string Username { get; set; }
        public string PasswordHash { get; set; }
        public string Phone { get; set; }
        public string? Email { get; set; }
        public UserType Kind { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public DateTime? UnregistrationDate { get; set; }
        public string CultureId { get; set; }
        public ulong ProviderId { get; set; }
        public LogEventType? LogLevel { get; set; }
        public virtual List<RefreshToken> RefreshTokens { get; set; } = new();
    }

    [ConventionalMap(typeof(UserEntity), direction: MapDirection.EntityToDto)]
    public class UserEntityDto : IEntity<long>
    {
        object IEntity.Id => Id;
        public long Id { get; set; }
        public string? Username { get; set; }
        public string Phone { get; set; }
        public string? Email { get; set; }
        [JsonConverter(typeof(JsonStringEnumConverter))]
        public UserType Kind { get; set; }
        public DateTime? RegistrationDate { get; set; }
        public DateTime? UnregistrationDate { get; set; }
        public string CultureId { get; set; }
        public ulong ProviderId { get; set; }
        public LogEventType? LogLevel { get; set; }

    }
}
