using System.Security.Cryptography;
using System.Text;

namespace HrManagement.Helpers
{
    public class PasswordHelper
    {
        public static string HashPassword(string password, string salt, int iterations = 1000, int hashSize = 32)
        {

            byte[] saltBytes = Convert.FromBase64String(salt);
            using (var pbkdf2 = new Rfc2898DeriveBytes(password, saltBytes, iterations, HashAlgorithmName.SHA256))
            {
                byte[] hash = pbkdf2.GetBytes(hashSize);
                return Convert.ToBase64String(hash);
            }
        }

        public static string GenerateSalt(int size = 32)
        {
            byte[] salt = new byte[size];
            RandomNumberGenerator.Fill(salt);
            return Convert.ToBase64String(salt);
        }

        public static bool VerifyPassword(string password, string salt, string hash)
        {
            string hashedPassword = HashPassword(password, salt);
            return hash == hashedPassword;
        }

        //public static string _HashPassword(string password)
        //{
        //    using (var sha256 = SHA256.Create())
        //    {
        //        byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(password));
        //        return Convert.ToBase64String(bytes);
        //    }
        //}

        //public static bool _VerifyPassword(string password, string storedHash)
        //{
        //    return _HashPassword(password) == storedHash;
        //}
    }
}
