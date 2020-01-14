using System;
using System.Linq;

namespace Text.UTF8.Legacy.Tests.Extensions
{
    public static class StringExtensions
    {
        /// <summary>
        /// Convert a byte array to hexadecimal
        /// </summary>
        /// <param name="bytes"></param>
        /// <param name="useDigitGrouping">True to seperate each hexadecimal value by a space</param>
        /// <returns></returns>
        public static string ToHexString(this byte[] bytes, bool useDigitGrouping = false)
        {
            var spacingChar = useDigitGrouping ? " " : string.Empty;
            return string.Concat(bytes.Select(b => b.ToString("X2") + spacingChar).ToArray()).Trim();
        }

        /// <summary>
        /// Convert a hex string to bytes
        /// </summary>
        /// <param name="hexString"></param>
        /// <param name="useSpacing">True to seperate each hexadecimal value by a space</param>
        /// <returns></returns>
        public static byte[] HexToBytes(this string hexString, bool useSpacing = false)
        {
            // performance is not a concern here, it's for tests.
            return Enumerable.Range(0, hexString.Length)
                     .Where(x => x % (useSpacing ? 3 : 2) == 0)
                     .Select(x => Convert.ToByte(hexString.Substring(x, 2), 16))
                     .ToArray();
        }
    }
}
