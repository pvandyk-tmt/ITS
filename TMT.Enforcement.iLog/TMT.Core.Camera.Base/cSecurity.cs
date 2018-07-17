#region

using System;
using System.Security.Cryptography;

#endregion

namespace TMT.Core.Camera.Base
{
    internal class cSecurity
    {
        /// <summary>
        /// 	Encodes a string into a MD5 scrambled string
        /// </summary>
        /// <param name = "plainText"></param>
        /// <returns></returns>
        public static string mD5Encoder(string plainText)
        {
            byte[] plainBytes = new byte[plainText.Length];
            for (int i = 0; i < plainText.Length; i++)
                plainBytes[i] = (byte) plainText[i];

            MD5 md5 = new MD5CryptoServiceProvider();

            byte[] result = md5.ComputeHash(plainBytes);

            string scrambledText = "";
            for (int j = 0; j < result.Length; j++)
                scrambledText += (char) result[j];

            return scrambledText;
        }

        public static byte[] getBytesToHash(byte[] imageBytes)
        {
            byte[] bytesToHash = new byte[20];
            int y = imageBytes.Length/20;
            int z = 0;
            for (int x = 0; x < imageBytes.Length && z < 20; x += y)
            {
                bytesToHash[z] = imageBytes[x];
                z++;
            }
            return bytesToHash;
        }
        
        public static string computeHash(byte[] bytesToHash)
        {
            byte[] saltBytes = new byte[4] {1, 2, 3, 4};

            // Convert plain text into a byte array.
            byte[] plainTextBytes = bytesToHash;

            // Allocate array, which will hold plain text and salt.
            byte[] plainTextWithSaltBytes =
                new byte[plainTextBytes.Length + saltBytes.Length];

            // Copy plain text bytes into resulting array.
            for (int i = 0; i < plainTextBytes.Length; i++)
                plainTextWithSaltBytes[i] = plainTextBytes[i];

            // Append salt bytes to the resulting array.
            for (int i = 0; i < saltBytes.Length; i++)
                plainTextWithSaltBytes[plainTextBytes.Length + i] = saltBytes[i];

            HashAlgorithm hash = new SHA1Managed();
            // Compute hash value of our plain text with appended salt.
            byte[] hashBytes = hash.ComputeHash(plainTextWithSaltBytes);

            // Create array which will hold hash and original salt bytes.
            byte[] hashWithSaltBytes = new byte[hashBytes.Length +
                                                saltBytes.Length];

            // Copy hash bytes into resulting array.
            for (int i = 0; i < hashBytes.Length; i++)
                hashWithSaltBytes[i] = hashBytes[i];

            // Append salt bytes to the result.
            for (int i = 0; i < saltBytes.Length; i++)
                hashWithSaltBytes[hashBytes.Length + i] = saltBytes[i];

            // Convert result into a base64-encoded string.
            string hashValue = Convert.ToBase64String(hashWithSaltBytes);

            // Return the result.
            return hashValue;
        }
    }
}