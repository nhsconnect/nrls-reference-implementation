using Hl7.Fhir.Model;
using Hl7.Fhir.Rest;
using Hl7.Fhir.Specification.Source;
using NRLS_API.Core.Interfaces.Helpers;
using System.IO;

namespace NRLS_API.Core.Helpers
{
    public class FhirCacheHelper : IFhirCacheHelper
    {
        private static IResourceResolver _source { get; set; }

        public FhirCacheHelper()
        {
            GetSource();
        }

        public Resource GetResourceProfile(string profileUrl)
        {
            return _source.ResolveByUri(profileUrl);
        }

        public ValueSet GetValueSet(string uri)
        {
            return _source.FindValueSet(uri);
        }

        public IResourceResolver GetSource()
        {

            if(_source == null)
            {
                _source = GetResolver();
            }

            return _source;
        }

        private IResourceResolver GetResolver()
        {
            var basePath = DirectoryHelper.GetBaseDirectory();

            var zip = Path.Combine(basePath, "Data", "definitions.xml.zip");

            return new CachedResolver(new MultiResolver(new WebResolver(uri => new FhirClient("https://fhir.nhs.uk/STU3")), new ZipSource(zip)));
        }
    }
}
