using Microsoft.AspNetCore.WebUtilities;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Demonstrator.Models.Extensions
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
                return new List<Tuple<string, string>>();
            }

            return QueryHelpers.ParseQuery(query).SelectMany(x => x.Value, (col, value) => new Tuple<string, string>(col.Key, value)).ToList();
        }

        public static IEnumerable<Tuple<string, string>> Cleaned(this IEnumerable<Tuple<string, string>> parameters)
        {
            if (parameters == null)
            {
                return null;
            }

            return parameters.Where(x => !x.Item1.Equals("_format"));
        }
    }
}
