using Hl7.Fhir.Rest;
using Hl7.Fhir.Specification.Source;
using System.IO;

namespace NRLS_API.Core.Helpers
{
    public static class FhirCacheHelper
    {
        private static IResourceResolver _source { get; set; }

        public static IResourceResolver Source
        {
            get
            {
                if(_source == null)
                {
                    _source = GetResolver();
                }

                return _source;
            }
        }

        private static IResourceResolver GetResolver()
        {
            var basePath = DirectoryHelper.GetBaseDirectory();

            var zip = Path.Combine(basePath, "Data\\definitions.xml.zip");

            return new CachedResolver(new MultiResolver(new WebResolver(uri => new FhirClient("https://fhir.nhs.uk/STU3")), new ZipSource(zip)));
        }
    }
}
