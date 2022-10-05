using Microsoft.Extensions.Options;
using Warehouse.Core.Domain.Entities;
using Warehouse.Core.Domain.Entities.Security;
using Warehouse.Core.Persistence;

namespace Warehouse.Core.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly IUserRepository _userRepository;

        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtUtils;
        private readonly AppSettings _appSettings;

        public AuthenticationService(
            IPasswordHasher passwordHasher,
            IJwtService jwtUtils,
            IOptions<AppSettings> appSettings,
            IUserRepository userRepository)
        {
            _passwordHasher = passwordHasher;
            _jwtUtils = jwtUtils;
            _userRepository = userRepository;
            _appSettings = appSettings.Value;
        }

        public async Task<AuthenticationResult> AuthenticateAsync(string username, string password, string ipAddress, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FindByNameAsync(username, cancellationToken);
            if (user is null || !_passwordHasher.VerifyHashedPassword(user.PasswordHash, password))
                throw new ApplicationException("Username or password is incorrect");

            // authentication successful so generate jwt and refresh tokens
            var roles = new List<RoleDTO>();
            if (_userRepository is IUserRoleStore roleStore)
            {
                roles.AddRange(await roleStore.GetUserRolesAsync(user.Id, cancellationToken));
            }
            var jwtToken = _jwtUtils.GenerateJwtToken(user, roles);
            var refreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
            user.RefreshTokens.Add(refreshToken);

            // remove old refresh tokens from user
            RemoveOldRefreshTokens(user);

            // save changes to db
            await _userRepository.UpdateAsync(user, cancellationToken);

            return new AuthenticationResult(user, roles, jwtToken, refreshToken.Token, refreshToken.Expires);
        }

        public async Task<AuthenticationResult> RefreshTokenAsync(string token, string ipAddress, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FindByRefreshTokenAsync(token, cancellationToken);
            if (user is null)
                throw new ApplicationException("Invalid token");

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);
            if (refreshToken.IsRevoked)
            {
                // revoke all descendant tokens in case this token has been compromised
                RevokeDescendantRefreshTokens(refreshToken, user, ipAddress, $"Attempted reuse of revoked ancestor token: {token}");
                // save changes to db
                await _userRepository.UpdateAsync(user, cancellationToken);
            }

            if (!refreshToken.IsActive)
                throw new ApplicationException("Invalid token");

            // replace old refresh token with a new one (rotate token)
            var newRefreshToken = RotateRefreshToken(refreshToken, ipAddress);
            user.RefreshTokens.Add(newRefreshToken);

            // remove old refresh tokens from user
            RemoveOldRefreshTokens(user);
            // save changes to db
            await _userRepository.UpdateAsync(user, cancellationToken);

            // generate new jwt
            var roles = new List<RoleDTO>();
            if (_userRepository is IUserRoleStore roleStore)
            {
                roles.AddRange(await roleStore.GetUserRolesAsync(user.Id, cancellationToken));
            }
            var jwtToken = _jwtUtils.GenerateJwtToken(user, roles);

            return new AuthenticationResult(user, roles, jwtToken, newRefreshToken.Token, newRefreshToken.Expires);
        }

        public async Task RevokeTokenAsync(string token, string ipAddress, CancellationToken cancellationToken)
        {
            var user = await _userRepository.FindByRefreshTokenAsync(token, cancellationToken);
            if (user is null)
                throw new ApplicationException("Invalid token");

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);
            if (!refreshToken.IsActive)
                throw new ApplicationException("Invalid token");

            // revoke token and save
            RevokeRefreshToken(refreshToken, ipAddress, "Revoked without replacement");
            // save changes to db
            await _userRepository.UpdateAsync(user, cancellationToken);
        }

        private RefreshToken RotateRefreshToken(RefreshToken refreshToken, string ipAddress)
        {
            var newRefreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
            RevokeRefreshToken(refreshToken, ipAddress, "Replaced by new token", newRefreshToken.Token);
            return newRefreshToken;
        }

        private void RemoveOldRefreshTokens(IUser user)
        {
            // remove old inactive refresh tokens from user based on TTL in app settings
            user.RefreshTokens.RemoveAll(x =>
                !x.IsActive &&
                x.Created.AddDays(_appSettings.RefreshTokenTTL) <= DateTime.UtcNow);
        }

        private static void RevokeDescendantRefreshTokens(RefreshToken refreshToken, IUser user, string ipAddress, string reason)
        {
            // recursively traverse the refresh token chain and ensure all descendants are revoked
            if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
            {
                var childToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken.ReplacedByToken);
                if (childToken is { IsActive: true })
                    RevokeRefreshToken(childToken, ipAddress, reason);
                else if (childToken is not null)
                    RevokeDescendantRefreshTokens(childToken, user, ipAddress, reason);
            }
        }

        private static void RevokeRefreshToken(RefreshToken token, string ipAddress, string reason = null, string replacedByToken = null)
        {
            token.Revoked = DateTime.UtcNow;
            token.RevokedByIp = ipAddress;
            token.ReasonRevoked = reason;
            token.ReplacedByToken = replacedByToken;
        }
    }
}
