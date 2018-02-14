using System;

namespace Demonstrator.Utilities.Exceptions
{
    public class InvalidEnumException : Exception
    {
        public InvalidEnumException(string enumType, string value) : base($"The value {value} is an invalid enum of type {enumType}") { }
    }
}
