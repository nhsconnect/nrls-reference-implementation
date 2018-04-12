using System;
using System.Collections.Generic;

namespace NRLS_API.Core.Helpers
{
    public static class EnumHelpers
    {
        public static bool IsValidName<T>(string value)
        {
            var enums = Enum.GetNames(typeof(T));

            return new List<string>(enums).Contains(value);
        }

    }
}
