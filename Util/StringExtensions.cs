using System;
using System.Text;
namespace ValorantManager.Util
{
    public static class StringExtensions
    {
        public static string FirstCharToUpper(this string input) =>
            input switch
            {
                null => throw new ArgumentNullException(nameof(input)),
                "" => throw new ArgumentException($"{nameof(input)} cannot be empty", nameof(input)),
                _ => string.Concat(input[0].ToString().ToUpper(), input.AsSpan(1))
            };

        public static string Base64Encode(this string input) => Convert.ToBase64String(Encoding.UTF8.GetBytes(input));
        public static string Base64Decode(this string input) => Encoding.UTF8.GetString(Convert.FromBase64String(input));
    }
}
