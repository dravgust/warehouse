using Vayosoft.WebAPI.Entities;
using Vayosoft.WebAPI.Models;

namespace Vayosoft.WebAPI.Services
{
    public interface IUserService
    {
        AuthenticateResponse Authenticate(AuthenticateRequest model, string ipAddress);
        AuthenticateResponse RefreshToken(string token, string ipAddress);
        void RevokeToken(string token, string ipAddress);
        IIdentityUser GetById(object id);
    }
}
