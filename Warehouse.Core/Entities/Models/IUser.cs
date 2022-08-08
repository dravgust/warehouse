using System.Text.Json.Serialization;
using Vayosoft.Core.SharedKernel.Entities;
using Warehouse.Core.Entities.Enums;

namespace Warehouse.Core.Entities.Models
{
    public interface IUser : IEntity
    {
        public string Username { get; }
        public string Email { get; }
        public UserType Kind { get; set; }

        [JsonIgnore]
        public string PasswordHash { get; }

        [JsonIgnore]
        public List<RefreshToken> RefreshTokens { get; }
    }
}
