using Microsoft.Extensions.Options;
using Warehouse.Core.Entities.Models;
using Warehouse.Core.Persistence;
using Warehouse.Core.UseCases.Administration.Models;

namespace Warehouse.Core.Services
{
    public class IdentityUserService : IIdentityUserService
    {
        private readonly IIdentityUserStore _identityUserStore;

        private readonly IPasswordHasher _passwordHasher;
        private readonly IJwtService _jwtUtils;
        private readonly AppSettings _appSettings;

        public IdentityUserService(
            IPasswordHasher passwordHasher,
            IJwtService jwtUtils,
            IOptions<AppSettings> appSettings,
            IIdentityUserStore identityUserStore)
        {
            _passwordHasher = passwordHasher;
            _jwtUtils = jwtUtils;
            _identityUserStore = identityUserStore;
            _appSettings = appSettings.Value;
        }

        public AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress)
        {
            var user = _identityUserStore.FindUserByNameAsync(model.Email);
            // validate
            if (user == null || !_passwordHasher.VerifyHashedPassword(user.PasswordHash, model.Password))
                throw new ApplicationException("Username or password is incorrect");

            // authentication successful so generate jwt and refresh tokens
            var jwtToken = _jwtUtils.GenerateJwtToken(user);
            var refreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
            user.RefreshTokens.Add(refreshToken);

            // remove old refresh tokens from user
            RemoveOldRefreshTokens(user);

            // save changes to db
            _identityUserStore.UnitOfWork.Add(user);
            _identityUserStore.UnitOfWork.Commit();

            return new AuthenticateResponse(user, jwtToken, refreshToken.Token, refreshToken.Expires);
        }

        public AuthenticateResponse RefreshToken(string token, string ipAddress)
        {
            var user = _identityUserStore.GetUserByRefreshToken(token);
            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (refreshToken.IsRevoked)
            {
                // revoke all descendant tokens in case this token has been compromised
                RevokeDescendantRefreshTokens(refreshToken, user, ipAddress, $"Attempted reuse of revoked ancestor token: {token}");
                _identityUserStore.UnitOfWork.Add(user);
                _identityUserStore.UnitOfWork.Commit();
            }

            if (!refreshToken.IsActive)
                throw new ApplicationException("Invalid token");

            // replace old refresh token with a new one (rotate token)
            var newRefreshToken = RotateRefreshToken(refreshToken, ipAddress);
            user.RefreshTokens.Add(newRefreshToken);

            // remove old refresh tokens from user
            RemoveOldRefreshTokens(user);

            // save changes to db
            _identityUserStore.UnitOfWork.Add(user);
            _identityUserStore.UnitOfWork.Commit();

            // generate new jwt
            var jwtToken = _jwtUtils.GenerateJwtToken(user);

            return new AuthenticateResponse(user, jwtToken, newRefreshToken.Token, newRefreshToken.Expires);
        }

        public void RevokeToken(string token, string ipAddress)
        {
            var user = _identityUserStore.GetUserByRefreshToken(token);
            var refreshToken = user.RefreshTokens.Single(x => x.Token == token);

            if (!refreshToken.IsActive)
                throw new ApplicationException("Invalid token");

            // revoke token and save
            RevokeRefreshToken(refreshToken, ipAddress, "Revoked without replacement");
            _identityUserStore.UnitOfWork.Add(user);
            _identityUserStore.UnitOfWork.Commit();
        }

        public IIdentityUser GetById(object id)
        {
            return _identityUserStore.GetById(id);
        }

        private RefreshToken RotateRefreshToken(RefreshToken refreshToken, string ipAddress)
        {
            var newRefreshToken = _jwtUtils.GenerateRefreshToken(ipAddress);
            RevokeRefreshToken(refreshToken, ipAddress, "Replaced by new token", newRefreshToken.Token);
            return newRefreshToken;
        }

        private void RemoveOldRefreshTokens(IIdentityUser user)
        {
            // remove old inactive refresh tokens from user based on TTL in app settings
            user.RefreshTokens.RemoveAll(x =>
                !x.IsActive &&
                x.Created.AddDays(_appSettings.RefreshTokenTTL) <= DateTime.UtcNow);
        }

        private static void RevokeDescendantRefreshTokens(RefreshToken refreshToken, IIdentityUser user, string ipAddress, string reason)
        {
            // recursively traverse the refresh token chain and ensure all descendants are revoked
            if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
            {
                var childToken = user.RefreshTokens.SingleOrDefault(x => x.Token == refreshToken.ReplacedByToken);
                if (childToken is {IsActive: true})
                    RevokeRefreshToken(childToken, ipAddress, reason);
                else
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
