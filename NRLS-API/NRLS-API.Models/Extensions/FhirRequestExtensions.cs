using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace NRLS_API.Models.Extensions
{
    public static class FhirRequestExtensions
    {
        public static string GetParameter(this IEnumerable<Tuple<string, string>> parameters, string key)
        {
            return parameters.FirstOrDefault(p => !string.IsNullOrEmpty(p.Item1) && p.Item1.Equals(key))?.Item2;
        }

        public static IEnumerable<Tuple<string, string>> GetParameters(this string query)
        {
            if (string.IsNullOrEmpty(query))
            {
                return null;
            }

            return QueryHelpers.ParseQuery(query).SelectMany(x => x.Value, (col, value) => new Tuple<string, string>(col.Key, value)).ToList();
        }
    }
}
