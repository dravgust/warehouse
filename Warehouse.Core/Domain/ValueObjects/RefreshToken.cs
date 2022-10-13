﻿using System.Text.Json.Serialization;
using Warehouse.Core.Domain.Entities.Identity;

namespace Warehouse.Core.Domain.ValueObjects
{
    public class RefreshToken
    {
        [JsonIgnore]
        public virtual IUser User { get; set; }
        [JsonIgnore]
        public long UserId { get; set; }

        public string Token { get; set; }
        public DateTime Expires { get; set; }
        public DateTime Created { get; set; }
        public string CreatedByIp { get; set; }
        public DateTime? Revoked { get; set; }
        public string RevokedByIp { get; set; }
        public string ReplacedByToken { get; set; }
        public string ReasonRevoked { get; set; }
        public bool IsExpired => DateTime.UtcNow >= Expires;
        public bool IsRevoked => Revoked != null;
        public bool IsActive => !IsRevoked && !IsExpired;
    }
}
