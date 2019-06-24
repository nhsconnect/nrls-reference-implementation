using System;
using System.Text;
using System.Text.RegularExpressions;

namespace Demonstrator.Utilities
{
    public class StringHelper
    {
        public static string CleanInput(string input)
        {
            if (string.IsNullOrEmpty(input))
            {
                return "";
            }

            var rgx = new Regex(@"[^A-Za-z0-9_\-/\s;:.@#~£$&%!'\+\(\)\[\]\*\?,>]");

            return rgx.Replace(input, "");
        }

        public static string UrlString(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return null;
            }

            var clean = Regex.Replace(value, "\\s+", "-");

            clean = Regex.Replace(clean, "-+", "-");

            return Regex.Replace(clean, @"[^a-zA-Z0-9-]", "");
        }

        //TODO: Review Microsoft.IdentityModel.Tokens Deserializers
        public static string Base64UrlDecode(string input)
        {
            input = input.Replace('-', '+').Replace('_', '/');

            input = input.PadRight(input.Length + (4 - input.Length % 4) % 4, '=');

            byte[] output = Convert.FromBase64String(input);

            return Encoding.UTF8.GetString(output);
        }
    }
}
