using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Specification.Source;
using Hl7.Fhir.Validation;
using NRLS_API.Core.Interfaces.Helpers;
using System.Collections.Generic;
using System.IO;

namespace NRLS_API.Core.Helpers
{
    public class FhirResourceHelper : IFhirResourceHelper
    {
        private static IResourceResolver _source { get; set; }

        public Validator Validator { get; }

        public FhirResourceHelper()
        {
            if (_source == null)
            {
                _source = GetResolver();
            }

            var ctx = new ValidationSettings()
            {
                ResourceResolver = _source,
                GenerateSnapshot = true,
                EnableXsdValidation = false,
                Trace = false,
                ResolveExteralReferences = true
            };


            Validator = new Validator(ctx);
        }

        public OperationOutcome ValidateResource<T>(T resource, string resourceSchema) where T : Resource
        {
            var customProfiles = new List<string>();

            if (!string.IsNullOrEmpty(resourceSchema))
            {
                customProfiles.Add(resourceSchema);
            }

            var result = Validator.Validate(resource, customProfiles.ToArray());

            return result;
        }

        public Resource GetResourceProfile(string profileUrl)
        {
            return _source.ResolveByUri(profileUrl);
        }

        public ValueSet GetValueSet(string uri)
        {
            return _source.FindValueSet(uri);
        }

        private IResourceResolver GetResolver()
        {
            var basePath = DirectoryHelper.GetBaseDirectory();

            var zip = Path.Combine(basePath, "Data", "definitions.xml.zip");
            var local = Path.Combine(basePath, "Data", "reference");

            return new CachedResolver(new MultiResolver(new DirectorySource(local, new DirectorySourceSettings { IncludeSubDirectories = true }), new WebResolver(uri => new FhirClient("https://fhir.nhs.uk/STU3")), new ZipSource(zip)));
        }
    }
}
