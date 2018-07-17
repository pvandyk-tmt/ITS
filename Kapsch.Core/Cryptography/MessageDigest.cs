using System;
using System.Security.Cryptography;
using System.Text;

namespace Kapsch.Core.Cryptography
{
    public class MessageDigest
    {
        public static string HashSHA256(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return value;

            using (var hash = SHA256Managed.Create())
            {
                return Convert.ToBase64String(hash.ComputeHash(Encoding.UTF8.GetBytes(value))).Replace("-", string.Empty).ToUpper();
            }
        } 
    }
}
