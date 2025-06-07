using CurConvApp.Models;

namespace CurConvApp.Repositories
{
    internal class UserRepository : IUserRepository
    {
        #region fields
        private readonly AppDbContext _context;
        #endregion

        public UserRepository(AppDbContext context)
        {
            _context = context;
        }

        public DbUser? GetByEmail(string email)
        {
            throw new NotImplementedException();
        }
        public void Add(User user)
        {
            throw new NotImplementedException();
        }
    }
}
