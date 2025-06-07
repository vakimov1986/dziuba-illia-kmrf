using CurConvApp.Models;

namespace CurConvApp.Repositories
{
    internal interface IUserRepository
    {
        DbUser? GetByEmail(string email);

        void Add(User user);
    }
}
