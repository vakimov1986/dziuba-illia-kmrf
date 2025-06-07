namespace SimpleLoginApp.Services
{
    internal interface IAuthenticator
    {
        bool VerifyPassword(string plainPassword, string passwordHash);

        string HashPassword(string plainPassword);
    }
}