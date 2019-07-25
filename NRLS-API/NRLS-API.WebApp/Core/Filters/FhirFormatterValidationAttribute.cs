using Hl7.Fhir.Model;
using Microsoft.AspNetCore.Mvc.Filters;
using NRLS_API.Core.Exceptions;
using NRLS_API.Core.Factories;
using System.Linq;
using System.Net;

namespace NRLS_API.WebApp.Core.Filters
{

    public class FhirFormatterValidationAttribute : ActionFilterAttribute
    {
        public override void OnActionExecuting(ActionExecutingContext context)
        {
            if (!context.ModelState.IsValid)
            {
                var error = context.ModelState.First();

                var message = error.Value.Errors.FirstOrDefault()?.ErrorMessage;

                OperationOutcome outcome = null;

                if (error.Key == "InputFormatter")
                {
                    outcome = OperationOutcomeFactory.CreateInvalidRequest();
                }
                else
                {
                    outcome = OperationOutcomeFactory.CreateInvalidResource(error.Key, message);
                }

                throw new HttpFhirException(message, outcome, HttpStatusCode.BadRequest);

            }
        }
    }
}
