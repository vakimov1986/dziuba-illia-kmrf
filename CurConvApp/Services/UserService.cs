using CurConvApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CurConvApp.Services
{
    public class UserService
    {
        private readonly AppDbContext _context;

        public UserService(AppDbContext context)
        {
            _context = context;
        }

        public bool RegisterUser(DbUser user)
        {
            if (_context.Users.Any(u => u.Email == user.Email))
                return false;

            user.PasswordHash = HashPassword(user.PasswordHash);
            _context.Users.Add(user);
            _context.SaveChanges();
            return true;
        }

        public DbUser? AuthenticateUser(string email, string password)
        {
            var user = _context.Users.FirstOrDefault(u => u.Email == email);
            if (user != null && user.PasswordHash == HashPassword(password))
                return user;
            return null;
        }

        private string HashPassword(string password)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }
}
