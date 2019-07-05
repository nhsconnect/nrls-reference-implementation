using NRLS_API.Models.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using Xunit;

namespace NRLS_APITest.Models.Extensions
{
    public class FhirRequestExtensionsTests
    {
        [Fact]
        public void FhirRequestExtensions_GetParameter()
        {
            var parameters = new List<Tuple<string, string>> { new Tuple<string, string>("_format", "json"), new Tuple<string, string>("subject", "nhsNumber") };

            var format = parameters.GetParameter("_format");

            Assert.Equal("json", format);
        }

        [Fact]
        public void FhirRequestExtensions_GetParameters()
        {
            var query = "?_format=json&custodian=https://fhir.nhs.uk/Id/ods-organization-code%7CXA999";
            var asParameters = query.GetParameters();

            Assert.Equal(2, asParameters.Count());
            Assert.Equal("json", asParameters.FirstOrDefault(x => x.Item1 == "_format").Item2);
            Assert.Equal("https://fhir.nhs.uk/Id/ods-organization-code|XA999", asParameters.FirstOrDefault(x => x.Item1 == "custodian").Item2);
        }

        [Fact]
        public void FhirRequestExtensions_GetParametersNull()
        {
            var query = string.Empty;
            var asParameters = query.GetParameters();

            Assert.NotNull(asParameters);
            Assert.Empty(asParameters);
        }

        [Fact]
        public void FhirRequestExtensions_Cleaned()
        {
            var query = "?_format=json&custodian=https://fhir.nhs.uk/Id/ods-organization-code%7CXA999";
            var asParameters = query.GetParameters().Cleaned();

            Assert.Single(asParameters);
            Assert.Equal("https://fhir.nhs.uk/Id/ods-organization-code|XA999", asParameters.FirstOrDefault(x => x.Item1 == "custodian").Item2);
        }

        [Fact]
        public void FhirRequestExtensions_CleanedNull()
        {
            IEnumerable<Tuple<string, string>> asParameters = null;

            Assert.Null(asParameters.Cleaned());
        }

    }
}
