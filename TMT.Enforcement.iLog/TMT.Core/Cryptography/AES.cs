using System;
using System.IO;
using System.Linq;
using System.Security.Cryptography;

namespace TMT.Core.Cryptography
{
    public class AES
    {
        private readonly int _saltSize = 32;

        public string Encrypt(string plainText, string key)
        {
            if (string.IsNullOrWhiteSpace(plainText))
                throw new ArgumentNullException("plainText");

            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException("key");

            using (var keyDerivationFunction = new Rfc2898DeriveBytes(key, _saltSize))
            {
                var saltBytes = keyDerivationFunction.Salt;
                var keyBytes = keyDerivationFunction.GetBytes(32);
                var ivBytes = keyDerivationFunction.GetBytes(16);

                using (var aesManaged = new AesManaged())
                {
                    aesManaged.KeySize = 256;

                    using (var encryptor = aesManaged.CreateEncryptor(keyBytes, ivBytes))
                    {
                        using (var memoryStream = new MemoryStream())
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
                            {
                                using (var streamWriter = new StreamWriter(cryptoStream))
                                {
                                    streamWriter.Write(plainText);
                                }
                            }

                            var cipherTextBytes = memoryStream.ToArray();
                            Array.Resize(ref saltBytes, saltBytes.Length + cipherTextBytes.Length);
                            Array.Copy(cipherTextBytes, 0, saltBytes, _saltSize, cipherTextBytes.Length);

                            return Convert.ToBase64String(saltBytes);
                        }  
                    }
                }
            }
        }

        public string Decrypt(string ciphertext, string key)
        {
            if (string.IsNullOrWhiteSpace(ciphertext))
                throw new ArgumentNullException("ciphertext");
            
            if (string.IsNullOrWhiteSpace(key))
                throw new ArgumentNullException("key");
            
            var allTheBytes = Convert.FromBase64String(ciphertext);
            var saltBytes = allTheBytes.Take(_saltSize).ToArray();
            var ciphertextBytes = allTheBytes.Skip(_saltSize).Take(allTheBytes.Length - _saltSize).ToArray();

            using (var keyDerivationFunction = new Rfc2898DeriveBytes(key, saltBytes))
            {
                var keyBytes = keyDerivationFunction.GetBytes(32);
                var ivBytes = keyDerivationFunction.GetBytes(16);

                using (var aesManaged = new AesManaged())
                {
                    using (var decryptor = aesManaged.CreateDecryptor(keyBytes, ivBytes))
                    {
                        using (var memoryStream = new MemoryStream(ciphertextBytes))
                        {
                            using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
                            {
                                using (var streamReader = new StreamReader(cryptoStream))
                                {
                                    return streamReader.ReadToEnd();
                                }
                            }
                        }
                    }
                }
            }
        }
    }
}
