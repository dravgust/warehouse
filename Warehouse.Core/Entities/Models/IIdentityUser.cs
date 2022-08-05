using System.Text.Json.Serialization;
using Vayosoft.Core.SharedKernel.Entities;

namespace Warehouse.Core.Entities.Models
{
    public interface IIdentityUser : IEntity
    {
        public string Username { get; }
        public string Email { get; }

        [JsonIgnore]
        public string PasswordHash { get; }

        [JsonIgnore]
        public List<RefreshToken> RefreshTokens { get; }
    }

    public interface IProviderable
    {
        object ProviderId { get; }
    }

    public interface IProviderable<out TKey> : IProviderable
    {
        new TKey ProviderId { get; }
    }
}
