using System.Text.Json.Serialization;
using Vayosoft.Core.SharedKernel.Entities;
using Warehouse.Core.Domain.Enums;

namespace Warehouse.Core.Domain.Entities
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
