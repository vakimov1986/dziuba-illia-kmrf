using CurConvApp.Models;

namespace CurConvApp.Services.Authentification
{
    public interface IAuthService
    {
        LoginResult Register(User user);

        LoginResult Authenticate(string email, string password);
    }
}
