using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CurConvApp.Services
{
    public static class PasswordHelper
    {
        // Перевірка пароля (універсальна)
        public static bool VerifyPassword(string enteredPassword, string storedHash)
        {
            if (string.IsNullOrEmpty(storedHash))
                return false;

            // Якщо BCrypt
            if (storedHash.StartsWith("$2"))
                return BCrypt.Net.BCrypt.Verify(enteredPassword, storedHash);

            // Якщо SHA256
            string enteredHash = HashPasswordSHA256(enteredPassword);
            return storedHash == enteredHash;
        }

        // Генерація BCrypt хеша для нових паролів
        public static string HashPassword(string password)
            => BCrypt.Net.BCrypt.HashPassword(password);

        // Генерація SHA256 хеша для старих паролів (для перевірки)
        public static string HashPasswordSHA256(string password)
        {
            using var sha = System.Security.Cryptography.SHA256.Create();
            var bytes = sha.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
            return Convert.ToBase64String(bytes);
        }
    }

}
