using System.Collections.Generic;
using System.Text.Json.Serialization;
using Vayosoft.Core.SharedKernel.Entities;

namespace Vayosoft.WebAPI.Entities
{
    public interface IIdentityUser: IEntity
    {
        public string Username { get; set; }
        public string Email { get; set; }

        [JsonIgnore]
        public string PasswordHash { get; set; }

        [JsonIgnore]
        public List<RefreshToken> RefreshTokens { get; set; }
    }
}
