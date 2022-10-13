using System.Text.Json.Serialization;
using Vayosoft.Core.SharedKernel.Entities;
using Warehouse.Core.Domain.Enums;
using Warehouse.Core.Domain.ValueObjects;

namespace Warehouse.Core.Domain.Entities.Identity
{
    public interface IUser : IEntity
    {
        public string Username { get; }
        public string Email { get; }
        public UserType Type { get; set; }

        [JsonIgnore]
        public List<RefreshToken> RefreshTokens { get; }
    }
}
