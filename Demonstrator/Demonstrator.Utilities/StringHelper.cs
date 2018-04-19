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
    }
}
