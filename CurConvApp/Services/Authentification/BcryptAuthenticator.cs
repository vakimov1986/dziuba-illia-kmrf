using SimpleLoginApp.Services;

namespace CurConvApp.Services.Authentification
{
    internal class BcryptAuthenticator : IAuthenticator
    {
        public string HashPassword(string rawPassword)
        {
            throw new NotImplementedException();
        }

        public bool VerifyPassword(string rawPassword, string passwordHash)
        {
            throw new NotImplementedException();
        }
    }
}
