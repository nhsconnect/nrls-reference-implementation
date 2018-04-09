using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Specification.Source;
using Hl7.Fhir.Validation;
using System.Collections.Generic;
using System.IO;

namespace NRLS_API.Core.Helpers
{
    public class ProfileHelper
    {

        public IResourceResolver Resolver { get; }

        public Validator Validator { get; }

        public ProfileHelper()
        {

            var basePath = DirectoryHelper.GetBaseDirectory();

            var zip = Path.Combine(basePath, "Data\\definitions.xml.zip");

            Resolver = new CachedResolver(new MultiResolver(new WebResolver(uri => new FhirClient("https://fhir.nhs.uk/STU3")), new ZipSource(zip)));

            var ctx = new ValidationSettings()
            {
                ResourceResolver = Resolver,
                GenerateSnapshot = true,
                EnableXsdValidation = false,
                Trace = false,
                ResolveExteralReferences = true
            };


            Validator = new Validator(ctx);
        }

        public bool ValidProfile<T>(T resource, string customProfile) where T : Resource
        {
            var customProfiles = new List<string>();

            if (!string.IsNullOrEmpty(customProfile))
            {
                customProfiles.Add(customProfile);
            }

            var result = Validator.Validate(resource, customProfiles.ToArray());

            return result.Success && result.Errors == 0;
        }
    }
}
