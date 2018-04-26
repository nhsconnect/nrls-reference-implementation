using System;

namespace Demonstrator.NRLSAdapter.Helpers.Exceptions
{
    public class InvalidResourceException : Exception
    {

        public InvalidResourceException(string type) : base($"The requested resource type of {type} is not valid.") { }

    }
}
