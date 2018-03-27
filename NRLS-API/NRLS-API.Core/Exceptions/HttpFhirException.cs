using Hl7.Fhir.Model;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;

namespace NRLS_API.Core.Exceptions
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

        public HttpFhirException(string message, OperationOutcome operationOutcome, Exception innerException = null) : base(message, innerException)
        {
            OperationOutcome = operationOutcome;
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
