using System.Linq;
using System.Text.RegularExpressions;

namespace Toppler.Extensions
{
    public static class StringExtensions
    {
        private static Regex alphaRegex = new Regex("^[a-zA-Z0-9.:]*$", RegexOptions.Compiled | RegexOptions.Singleline);
        private const string separator = ":";

        /// <summary>
        /// Check if string is ONLY alphanumeric (regex ^[a-zA-Z0-9]*$)
        /// </summary>
        /// <param name="s"></param>
        /// <returns></returns>
        public static bool AlphaNumericString(this string s)
        {
            return alphaRegex.IsMatch(s);
        }

        /// <summary>
        /// Generate A Redis Key using ':' (semi colon) as delimiter.
        /// </summary>
        /// <param name="parts"></param>
        /// <returns></returns>
        public static string AsRedisKey(this string[] parts)
        {
            return string.Join(separator, parts.Where(p => !string.IsNullOrWhiteSpace(p)));
        }
    }
}
