using System;

namespace NRLS_API.Core.Exceptions
{
    public class InvalidEnumException : Exception
    {
        public InvalidEnumException(string enumType, string value) : base($"The value {value} is an invalid enum of type {enumType}") { }
    }
}

