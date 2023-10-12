using System.Security.Cryptography;

namespace Weather.Api.Utils
{
    public static class PasswordHasher
    {
        public static string HashPassword(string password)
        {
            // Generate a salt (a random value used to enhance security)
            byte[] salt = new byte[16];
            using (RandomNumberGenerator rng = RandomNumberGenerator.Create())
            {
                rng.GetBytes(salt);
            }

            // Create a password hash using PBKDF2 with a specified number of iterations
            var pbkdf2 = new Rfc2898DeriveBytes(password, salt, 10000, HashAlgorithmName.SHA256);
            byte[] hash = pbkdf2.GetBytes(20); // 20 is the size of the resulting hash

            // Combine the salt and hash and convert to base64 for storage
            byte[] hashBytes = new byte[36]; // 16 (salt) + 20 (hash)
            Array.Copy(salt, 0, hashBytes, 0, 16);
            Array.Copy(hash, 0, hashBytes, 16, 20);
            return Convert.ToBase64String(hashBytes);
        }
    }
}
