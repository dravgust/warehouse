using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Security.Principal;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Warehouse.Core.Application.Services;
using Warehouse.Core.Application.Services.Authentication;
using Warehouse.Core.Domain.Entities;
using Warehouse.Core.Domain.Entities.Security;
using Warehouse.Core.Domain.Enums;

namespace Warehouse.Infrastructure.Authentication
{
    public class JwtService : IJwtService
    {
        private readonly AppSettings _appSettings;

        public JwtService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public string GenerateJwtToken(IUser user, IEnumerable<SecurityRoleEntity> roles)
        {
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var claims = new List<Claim>
            {
                new(ClaimTypes.NameIdentifier, user.Id.ToString()?? throw new InvalidOperationException("The User has no Id"), ClaimValueTypes.Integer64),
                new(ClaimTypes.Name, user.Username, ClaimValueTypes.String), 
                new(ClaimTypes.Email, user.Email ?? string.Empty, ClaimValueTypes.Email), 
                new(CustomClaimTypes.UserType, user.Type.ToString(), ClaimValueTypes.String),
                new(CustomClaimTypes.ProviderId, $"{(user as IProvider)?.ProviderId ?? 0}", ClaimValueTypes.Integer64)
            };

            claims.AddRange(roles.Select(role => new Claim(ClaimsIdentity.DefaultRoleClaimType, role.Id, ClaimValueTypes.String)));

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(Math.Max(_appSettings.TokenExpires,  1)),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public IPrincipal GetPrincipalFromJwtToken(string token)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            try
            {
                var principal = tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                if (validatedToken is not JwtSecurityToken jwtSecurityToken
                    || !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return new ClaimsPrincipal();
                }

                return principal;
            }
            catch
            {
                return new ClaimsPrincipal();
            }
        }

        public RefreshToken GenerateRefreshToken(string ipAddress)
        {
            var random = new byte[64];
            using var rng = RandomNumberGenerator.Create();

            var refreshToken = new RefreshToken
            {
                Token = GetUniqueToken(),
                Expires = DateTime.UtcNow.AddDays(Math.Max(_appSettings.RefreshTokenExpires, 1)),
                Created = DateTime.UtcNow,
                CreatedByIp = ipAddress
            };

            return refreshToken;

            string GetUniqueToken()
            {
                // token is a cryptographically strong random sequence of values
                rng.GetBytes(random);
                var token = Convert.ToBase64String(random);
                // ensure token is unique by checking against db
                //var tokenIsUnique = !_context.Users.Any(u => u.RefreshTokens.Any(t => t.Token == token));

                //if (!tokenIsUnique)
                // return GetUniqueToken();

                return token;
            }
        }
    }
}
