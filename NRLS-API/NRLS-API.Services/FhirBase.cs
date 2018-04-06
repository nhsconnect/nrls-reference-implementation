using Microsoft.Extensions.Options;
using NRLS_API.Core.Exceptions;
using NRLS_API.Core.Factories;
using NRLS_API.Models.Core;
using System.Collections.Generic;
using System.Linq;
using System.Net;

namespace NRLS_API.Services
{
    public class FhirBase
    {
        protected readonly IList<string> _supportedResources;

        public FhirBase(IOptions<NrlsApiSetting> nrlsApiSetting)
        {
            _supportedResources = nrlsApiSetting.Value.SupportedResources;
        }

        protected void ValidateResource(string resourceType)
        {
            if (_supportedResources.Any() && !_supportedResources.Contains(resourceType))
            {
                throw new HttpFhirException("Bad Request", OperationOutcomeFactory.CreateInvalidResource(resourceType), HttpStatusCode.BadRequest);
            }
        }
    }
}
