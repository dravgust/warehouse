using System.Text.Json.Serialization;
using Vayosoft.Core.SharedKernel.Entities;

namespace Warehouse.Core.Entities.Models
{
    public interface IUser : IEntity
    {
        public string Username { get; }
        public string Email { get; }

        [JsonIgnore]
        public string PasswordHash { get; }

        [JsonIgnore]
        public List<RefreshToken> RefreshTokens { get; }
    }
}
