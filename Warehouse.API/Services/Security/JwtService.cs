using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Options;
using Microsoft.IdentityModel.Tokens;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Services;
using Warehouse.Core.UseCases.Administration.Models;

namespace Warehouse.API.Services.Security
{
    public class JwtService : IJwtService
    {
        private readonly AppSettings _appSettings;

        public JwtService(IOptions<AppSettings> appSettings)
        {
            _appSettings = appSettings.Value;
        }

        public string GenerateJwtToken(IIdentityUser user)
        {
            // generate token that is valid for 15 minutes
            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            var claims = new[]
            {
                new Claim(ClaimTypes.Name, user.Username),
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString() ?? throw new InvalidOperationException("The User has no ID")),
                new Claim(nameof(IIdentityUser.Id), $"{user.Id}"),
                new Claim(nameof(IProviderable.ProviderId), $"{((IProviderable)user)?.ProviderId}")
            };

            var tokenDescriptor = new SecurityTokenDescriptor
            {
                Subject = new ClaimsIdentity(claims),
                Expires = DateTime.UtcNow.AddMinutes(30),
                SigningCredentials = new SigningCredentials(new SymmetricSecurityKey(key), SecurityAlgorithms.HmacSha256Signature)
            };
            var token = tokenHandler.CreateToken(tokenDescriptor);
            return tokenHandler.WriteToken(token);
        }

        public IdentityContext ValidateJwtToken(string token)
        {
            if (token == null)
                return null;

            var tokenHandler = new JwtSecurityTokenHandler();
            var key = Encoding.ASCII.GetBytes(_appSettings.Secret);
            try
            {
                tokenHandler.ValidateToken(token, new TokenValidationParameters
                {
                    ValidateIssuerSigningKey = true,
                    IssuerSigningKey = new SymmetricSecurityKey(key),
                    ValidateIssuer = false,
                    ValidateAudience = false,
                    // set clockskew to zero so tokens expire exactly at token expiration time (instead of 5 minutes later)
                    ClockSkew = TimeSpan.Zero
                }, out SecurityToken validatedToken);

                var jwtToken = (JwtSecurityToken)validatedToken;
                var userId = long.Parse(jwtToken.Claims.First(x => x.Type == nameof(IIdentityUser.Id)).Value);
                var providerId = long.Parse(jwtToken.Claims.First(x => x.Type == nameof(IProviderable.ProviderId)).Value);

                // return user id from JWT token if validation successful
                return new IdentityContext
                {
                    UserId = userId,
                    ProviderId = providerId
                };
            }
            catch
            {
                // return null if validation fails
                return null;
            }
        }

        public RefreshToken GenerateRefreshToken(string ipAddress)
        {
            var random = new byte[64];
            using var rng = RandomNumberGenerator.Create();

            var refreshToken = new RefreshToken
            {
                Token = GetUniqueToken(),
                // token is valid for 7 days
                Expires = DateTime.UtcNow.AddDays(7),
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
