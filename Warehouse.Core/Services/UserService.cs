using Microsoft.Extensions.Options;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Persistence;
using Warehouse.Core.UseCases.Administration.Models;

namespace Warehouse.Core.Services
{
    public class UserService : IUserService
    {
        private readonly IUserStore<UserEntity> _userStore;

        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtUtils;
        private readonly AppSettings _appSettings;

        public UserService(
            IPasswordHasher passwordHasher,
            IJwtService jwtUtils,
            IOptions<AppSettings> appSettings,
            IUserStore<UserEntity> userStore)
        {
            _passwordHasher = passwordHasher;
            _jwtUtils = jwtUtils;
            _userStore = userStore;
            _appSettings = appSettings.Value;
        }

        public async Task<AuthenticateResponse> AuthenticateAsync(AuthenticateRequest model, string ipAddress, CancellationToken cancellationToken)
        {
            var user = await _userStore.FindByNameAsync(model.Email, cancellationToken);
            if (user == null || !_passwordHasher.VerifyHashedPassword(user.PasswordHash, model.Password))
                throw new ApplicationException("Username or password is incorrect");

            // authentication successful so generate jwt and refresh tokens
            var roles = await ((IUserRoleStore) _userStore).GetUserRolesAsync(user.Id, cancellationToken);
            var jwtToken = _jwtUtils.GenerateJwtToken(user, roles);
            var refreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
            user.RefreshTokens.Add(refreshToken);

            // remove old refresh tokens from user
            RemoveOldRefreshTokens(user);

            // save changes to db
            await _userStore.UpdateAsync(user, cancellationToken);

            return new AuthenticateResponse(user, roles, jwtToken, refreshToken.Token, refreshToken.Expires);
        }

        public async Task<AuthenticateResponse> RefreshTokenAsync(string token, string ipAddress, CancellationToken cancellationToken)
        {
            var user = await _userStore.FindByRefreshTokenAsync(token, cancellationToken);
            if (user == null)
                throw new ApplicationException("Invalid token");

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);
            if (refreshToken.IsRevoked)
            {
                // revoke all descendant tokens in case this token has been compromised
                RevokeDescendantRefreshTokens(refreshToken, user, ipAddress, $"Attempted reuse of revoked ancestor token: {token}");
                // save changes to db
                await _userStore.UpdateAsync(user, cancellationToken);
            }

            if (!refreshToken.IsActive)
                throw new ApplicationException("Invalid token");

            // replace old refresh token with a new one (rotate token)
            var newRefreshToken = RotateRefreshToken(refreshToken, ipAddress);
            user.RefreshTokens.Add(newRefreshToken);

            // remove old refresh tokens from user
            RemoveOldRefreshTokens(user);
            // save changes to db
            await _userStore.UpdateAsync(user, cancellationToken);

            // generate new jwt
            var roles = await ((IUserRoleStore)_userStore).GetUserRolesAsync(user.Id, cancellationToken);
            var jwtToken = _jwtUtils.GenerateJwtToken(user, roles);

            return new AuthenticateResponse(user, roles, jwtToken, newRefreshToken.Token, newRefreshToken.Expires);
        }

        public async Task RevokeTokenAsync(string token, string ipAddress, CancellationToken cancellationToken)
        {
            var user = await _userStore.FindByRefreshTokenAsync(token, cancellationToken);
            if (user == null)
                throw new ApplicationException("Invalid token");

            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);
            if (!refreshToken.IsActive)
                throw new ApplicationException("Invalid token");

            // revoke token and save
            RevokeRefreshToken(refreshToken, ipAddress, "Revoked without replacement");
            // save changes to db
            await _userStore.UpdateAsync(user, cancellationToken);
        }

        public async Task<IUser> GetByUserNameAsync(string username, CancellationToken cancellationToken)
        {
            var user = await _userStore.FindByNameAsync(username, cancellationToken);
            if (user == null)
                throw new ApplicationException("Invalid user");
            return user;
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
                if (childToken is {IsActive: true})
                    RevokeRefreshToken(childToken, ipAddress, reason);
                else if(childToken != null)
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
