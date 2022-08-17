﻿using System.Text.Json.Serialization;
using Warehouse.Core.Entities.Models.Security;
using Warehouse.Core.Entities.Models;

namespace Warehouse.Core.UseCases.Administration.Models
{
    public class AuthenticateResult
    {
        public IUser User { get; set; }
        [JsonIgnore]
        public IList<RoleDTO> Roles { get; set; }
        public string Token { get; set; }
        public long TokenExpirationTime { get; set; }

        [JsonIgnore] // refresh token is returned in http only cookie
        public string RefreshToken { get; set; }

        public AuthenticateResult(IUser user, IList<RoleDTO> roles, string jwtToken, string refreshToken, DateTime expirationTime)
        {
            User = user;
            Roles = roles;
            Token = jwtToken;
            TokenExpirationTime = ((DateTimeOffset)expirationTime).ToUnixTimeSeconds();
            RefreshToken = refreshToken;
        }
    }
}