using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace DemonstratorTest.Comparer
{
    public static class Comparers
    {
        public static IEqualityComparer<T> ModelComparer<T>()
        {
            var fields = typeof(T).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            var actions = new List<Func<T, T, bool>>();


            //Assert.Equal(obj1Str, obj2Str);

            //foreach (var field in fields)
            //{
                    actions.Add((x, y) => {

                        var xToString = JsonConvert.SerializeObject(x);
                        var yToString = JsonConvert.SerializeObject(y);

                        return string.Equals(xToString, yToString);
                    });
            //}


            return new GenericComparer<T>(actions.ToArray());
        }
    }
}
