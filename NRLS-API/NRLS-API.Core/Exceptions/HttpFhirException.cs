using Hl7.Fhir.Model;
using System;
using System.Net;

namespace NRLS_API.Core.Exceptions
{
    public class HttpFhirException : Exception
    {
        public HttpFhirException() : base()
        {
            StatusCode = HttpStatusCode.InternalServerError;
        }

        public HttpFhirException(string message) : base(message)
        {
            StatusCode = HttpStatusCode.InternalServerError;
        }

        public HttpFhirException(string message, Exception innerException) : base(message, innerException)
        {
            StatusCode = HttpStatusCode.InternalServerError;
        }

        public HttpFhirException(string message, OperationOutcome operationOutcome, Exception innerException = null) : base(message, innerException)
        {
            OperationOutcome = operationOutcome;
            StatusCode = HttpStatusCode.InternalServerError;
        }

        public HttpFhirException(string message, OperationOutcome operationOutcome, HttpStatusCode statusCode) : base(message)
        {
            OperationOutcome = operationOutcome;
            StatusCode = statusCode;
        }

        public OperationOutcome OperationOutcome { get; set; }

        public HttpStatusCode StatusCode { get; set; }
    }
}
