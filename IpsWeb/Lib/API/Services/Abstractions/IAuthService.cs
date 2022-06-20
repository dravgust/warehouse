using IpsWeb.Lib.API.ViewModels;

namespace IpsWeb.Lib.API.Services.Abstractions
{
    public interface IAuthService
    {
        string HashPassword(string password);
        bool VerifyPassword(string actualPassword, string hashedPassword);
        AuthData GetAuthData(long id);
    }
}
