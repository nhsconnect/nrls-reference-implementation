using Demonstrator.Utilities.Exceptions;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Demonstrator.Utilities
{
    public static class EnumHelpers
    {
        public static bool IsValidName<T>(string value)
        {
            var enums = Enum.GetNames(typeof(T));

            return new List<string>(enums).Contains(value);
        }

        public static TEnum GetEnum<TEnum>(string name)
        {
            if (!IsValidName<TEnum>(name))
            {
                throw new InvalidEnumException(typeof(TEnum).Name, name);
            }

            TEnum enumName = (TEnum)Enum.Parse(typeof(TEnum), name);

            return enumName;
        }
    }
}
