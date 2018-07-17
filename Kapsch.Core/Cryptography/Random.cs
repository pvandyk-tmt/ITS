using System;
using System.Linq;
using System.Text;

namespace Kapsch.Core.Cryptography
{
    public class Random
    {
        public static string GenerateString(int size, bool lowerCase)
        {
            var builder = new StringBuilder();
            var random = new System.Random();
            char ch;

            for (int i = 1; i < size + 1; i++)
            {
                ch = Convert.ToChar(Convert.ToInt32(Math.Floor(26 * random.NextDouble() + 65)));
                builder.Append(ch);
            }

            if (lowerCase)
                return builder.ToString().ToLower();
            
            return builder.ToString();
        }

        public static string GenerateConcatenatedString(params string[] values)
        {
            var random = new System.Random();
            var currentValue = string.Empty;

            values.ToList().ForEach(f => currentValue = string.Format("{0}{1}", currentValue, f));

            return string.Format("{0}{1}", currentValue.Replace(" ", string.Empty), random.Next(0, 99).ToString("D2"));
        }
    }
}
