using Hl7.Fhir.Model;
using System;

namespace Demonstrator.Core.Exceptions
{
    public class HttpFhirException : Exception
    {

        public HttpFhirException() : base()
        {

        }

        public HttpFhirException(string message) : base(message)
        {

        }

        public HttpFhirException(string message, Exception innerException) : base(message, innerException)
        {

        }

        public HttpFhirException(string message, OperationOutcome operationOutcome, Exception innerException) : base(message, innerException)
        {
            OperationOutcome = operationOutcome;
        }

        public OperationOutcome OperationOutcome { get; set; }
    }
}
