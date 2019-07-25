﻿using Hl7.Fhir.Model;
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

        public string _resourceProfile { get; set; }

        public FhirBase(IOptionsSnapshot<ApiSetting> apiSetting, string optionName = null)
        {
            if(!string.IsNullOrEmpty(optionName))
            {
                var apiSettings = apiSetting.Get(optionName);
                _supportedResources = apiSettings.SupportedResources;
                _resourceProfile = apiSettings.ProfileUrl;
            }

        }

        protected void ValidateResource(string resourceType)
        {
            if (_supportedResources.Any() && !_supportedResources.Contains(resourceType))
            {
                throw new HttpFhirException("Bad Request", OperationOutcomeFactory.CreateInvalidResourceType(resourceType), HttpStatusCode.BadRequest);
            }
        }

        protected Resource ParseRead<T>(T results, string id) where T : Resource
        {
            var notFound = results == null;

            if (!notFound && results.ResourceType == ResourceType.Bundle)
            {
                var bundle = results as Bundle;
                notFound = (!bundle.Total.HasValue || bundle.Total.Value != 1);
            }

            if (notFound)
            {
                throw new HttpFhirException("Not Found", OperationOutcomeFactory.CreateNotFound(id), HttpStatusCode.NotFound);
            }

            return results;
        }
    }
}
