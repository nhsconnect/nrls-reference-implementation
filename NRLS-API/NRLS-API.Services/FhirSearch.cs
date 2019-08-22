using Hl7.Fhir.Model;
using Microsoft.Extensions.Options;
using MongoDB.Driver;
using NRLS_API.Core.Exceptions;
using NRLS_API.Core.Factories;
using NRLS_API.Core.Interfaces.Database;
using NRLS_API.Core.Interfaces.Helpers;
using NRLS_API.Core.Interfaces.Services;
using NRLS_API.Models.Core;
using NRLS_API.Services.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Threading.Tasks;

namespace NRLS_API.Services
{
    public class FhirSearch : FhirBase, IFhirSearch
    {
        private readonly INRLSMongoDBCaller _context;
        private readonly IFhirSearchHelper _fhirSearchHelper;

        public FhirSearch(IOptionsSnapshot<ApiSetting> apiSetting, INRLSMongoDBCaller context, IFhirSearchHelper fhirSearchHelper) : base(apiSetting, "NrlsApiSetting")
        {
            _context = context;
            _fhirSearchHelper = fhirSearchHelper;
        }

        public async Task<Bundle> GetAsBundle<T>(FhirRequest request) where T : Resource
        {
            var bundle = await Find<T>(request, true);

            return bundle;
        }

        public async Task<T> Get<T>(FhirRequest request) where T : Resource
        {
            //Force BuildQuery to return simple id filter
            request.IsIdQuery = true;

            var documents = await Find<T>(request, true);

            return documents.Entry.FirstOrDefault()?.Resource as T;
        }

        public async Task<Bundle> GetByMasterId<T>(FhirRequest request) where T : Resource
        {
            //Add identifier on the fly as it is not a standard search parameter
            request.AllowedParameters = request.AllowedParameters.Concat(new[] { "identifier" }).ToArray();

            var bundle = await Find<T>(request, true);

            return bundle;
        }

        public async Task<Bundle> Find<T>(FhirRequest request, bool returnFirst) where T : Resource
        {
            ValidateResource(request.StrResourceType);

            try
            {
                var query = _fhirSearchHelper.BuildQuery(request);

                var resources = await  _context.FindResource(request.StrResourceType, query).ToFhirListAsync<T>();

                var bundle = _fhirSearchHelper.ToBundle<T>(request, resources);

                if (returnFirst)
                {
                    var entries = bundle.Entry;
                    bundle.Entry = entries.Take(1).ToList();
                    bundle.Total = 1;
                }

                return bundle;
            }
            catch (Exception ex)
            {
                throw new HttpFhirException($"FhirSearch.Find | {ex.Message}", OperationOutcomeFactory.CreateInternalError(ex.Message), HttpStatusCode.BadRequest);
            }
        }      
    }
}
