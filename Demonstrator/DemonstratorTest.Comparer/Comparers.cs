using System;
using System.Collections.Generic;

namespace DemonstratorTest.Comparer
{
    public static class Comparers
    {
        public static IEqualityComparer<T> ModelComparer<T>()
        {
            var fields = typeof(T).GetFields(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            var actions = new List<Func<T, T, bool>>();

            foreach (var field in fields)
            {
                    actions.Add((x, y) => (field.GetValue(x) == null && field.GetValue(y) == null) ||
                                          (field.GetType() == typeof(string) && string.Equals(field.GetValue(x), field.GetValue(y))) ||  
                                          field.GetValue(x) == field.GetValue(y));
            }


            return new GenericComparer<T>(actions.ToArray());
        }
    }
}
