using CurConvApp.Models;
using CurConvApp.Repositories;
using SimpleLoginApp.Services;

namespace CurConvApp.Services.Authentification
{
    internal class AuthService : IAuthService
    {
        readonly IUserRepository _userRepositiry;
        readonly IAuthenticator _authenticator;

        public AuthService(IUserRepository userRepositiry, IAuthenticator authenticator)
        {
           _userRepositiry = userRepositiry;
           _authenticator = authenticator;
        }

        public LoginResult Authenticate(string username, string password)
        {
            var user = _userRepositiry.GetByEmail(username);
            if (user == null)
            {
                return LoginResult.Failed("User doesnt exist");
            }

            bool isPasswordValid = _authenticator.VerifyPassword(password, user.PasswordHash);

            if (!isPasswordValid)
            {
                return LoginResult.Failed("Invalid password");
            }

            return LoginResult.Success(
                new User() 
                {
                    Name = user.Name,
                    Surname = user.Surname,
                });
        }

        public LoginResult Register(User user)
        {
            //your implementation
            return LoginResult.Success(user);
        }
    }
}
