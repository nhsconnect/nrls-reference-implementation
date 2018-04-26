using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DemonstratorTest.Comparer
{
    public static class Comparers
    {
        public static IEqualityComparer<T> ModelComparer<T>()
        {
            var actions = new List<Func<T, T, bool>>();

            actions.Add((x, y) => {

                var xToString = JsonConvert.SerializeObject(x);
                var yToString = JsonConvert.SerializeObject(y);

                return string.Equals(xToString, yToString);
            });

            return new GenericComparer<T>(actions.ToArray());
        }
    }
}
