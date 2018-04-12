using System.Collections.Generic;

namespace Demonstrator.Utilities.Extensions
{
    public static class UriExt
    {
        public static IDictionary<string, string> ParseQuery(this string query)
        {
            var parameters = new Dictionary<string, string>();

            if (!string.IsNullOrEmpty(query))
            {
                var queryParameters = query.Substring(1).Split("&");

                foreach(var qp in queryParameters)
                {
                    var kvp = qp.Split("=");

                    if(kvp.Length == 2)
                    {
                        parameters.Add(kvp[0], kvp[1]);
                    }
                    else if (kvp.Length == 1)
                    {
                        parameters.Add(kvp[0], null);
                    }
                }
            }

            return parameters;
        }
    }
}
