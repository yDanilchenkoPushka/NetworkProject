using System.Text.RegularExpressions;

namespace Extensions
{
    public static class String
    {
        public static bool IsMatch(this string input, string pattern)
        {
            Regex regex = new Regex(pattern, 
                RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.IgnorePatternWhitespace);

            return regex.Match(input).Success;
        }
    }
}