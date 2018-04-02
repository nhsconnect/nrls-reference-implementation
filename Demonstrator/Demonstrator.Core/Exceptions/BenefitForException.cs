using System;

namespace Demonstrator.Core.Exceptions
{
    public class BenefitForException : Exception
    {

        public BenefitForException(string forType) : base($"The requested benefit for type of {forType} is not valid.") {}

    }
}
