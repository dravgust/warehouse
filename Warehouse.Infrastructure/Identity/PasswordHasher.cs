using System.Security.Cryptography;
using System.Text;
using Vayosoft.Core.Utilities;
using Warehouse.Core.Application.Common.Services;

namespace Warehouse.Infrastructure.Identity
{
    public class PasswordHasher : IPasswordHasher
    {
        public string HashPassword(string password)
        {
            return ReversePassword(password);
        }

        public bool VerifyHashedPassword(string hashedPassword, string providedPassword)
        {
            if (hashedPassword == ReversePassword(providedPassword))
            {
                return true;
            }

            return false;
        }

        protected virtual string ReversePassword(string value)
        {
            // SHA512 is disposable by inheritance.  
            using var sha256 = SHA256.Create();
            // Send a sample text to hash.  
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
            // Get the hashed string.  
            return BitConverter.ToString(hashedBytes).Replace("-", "").ToLower();
        }

        private static string GetSalt()
        {
            var bytes = new byte[128 / 8];
            using var keyGenerator = RandomNumberGenerator.Create();
            keyGenerator.GetBytes(bytes);
            return BitConverter.ToString(bytes).Replace("-", "").ToLower();
        }
    }

    public class MD5PasswordHasher : PasswordHasher
    {
        protected override string ReversePassword(string value)
        {
            using var sha256 = MD5.Create();
            var hashedBytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(value));
            return Base64Utils.EncodeBase64(hashedBytes);
        }
    }
}
