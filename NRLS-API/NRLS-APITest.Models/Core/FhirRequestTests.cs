using Hl7.Fhir.Model;
using NRLS_API.Models.Core;
using NRLS_APITest.Data;
using Xunit;

namespace NRLS_APITest.Models
{
    public class FhirRequestTests
    {
        [Fact]
        public void FhirRequest_ValidId()
        {
            var expectedId = "testId";

            var request = FhirRequest.Create("testId", ResourceType.DocumentReference, NrlsPointers.Valid, HttpContexts.Valid_Delete_Pointer.Request, "000");

            Assert.Equal(expectedId, request.Id);
        }

        [Fact]
        public void FhirRequest_NoId()
        {
            var request = FhirRequest.Create(null, ResourceType.DocumentReference, NrlsPointers.Valid, HttpContexts.Valid_Delete_Pointer.Request, "000");

            Assert.Null(request.Id);
        }

        [Fact]
        public void FhirRequest_ValidIdParam()
        {
            var expectedId = "testId";

            var request = FhirRequest.Create(null, ResourceType.DocumentReference, NrlsPointers.Valid, HttpContexts.Valid_Delete_Pointer.Request, "000");

            Assert.True(request.HasIdParameter);

            Assert.Equal(expectedId, request.IdParameter);
        }

        [Fact]
        public void FhirRequest_ValidHasNoIdParam()
        {
            var request = FhirRequest.Create(null, ResourceType.DocumentReference, null, HttpContexts.Valid_Create_Pointer.Request, "000");

            Assert.False(request.HasIdParameter);
        }

        [Fact]
        public void FhirRequest_ValidIdentifierParam()
        {
            var expected = "testSystem|testValue";

            var request = FhirRequest.Create(null, ResourceType.DocumentReference, null, HttpContexts.Valid_ConditionalDelete.Request, "000");

            Assert.Equal(expected, request.IdentifierParameter);
        }

        [Fact]
        public void FhirRequest_ValidSubjectParam()
        {
            var expected = "https://demographics.spineservices.nhs.uk/STU3/Patient/2686033207";

            var request = FhirRequest.Create(null, ResourceType.DocumentReference, null, HttpContexts.Valid_ConditionalDelete.Request, "000");

            Assert.Equal(expected, request.SubjectParameter);
        }

        [Fact]
        public void FhirRequest_ValidCreate_TwoParams()
        {
            var expectedId = "testId";

            var request = FhirRequest.Create("testId", ResourceType.DocumentReference);

            Assert.Equal(expectedId, request.Id);

            Assert.Equal(ResourceType.DocumentReference, request.ResourceType);

            Assert.Contains("subject", request.AllowedParameters);
            Assert.Contains("custodian", request.AllowedParameters);
            Assert.Contains("_id", request.AllowedParameters);
            Assert.Contains("type", request.AllowedParameters);
            Assert.Contains("_format", request.AllowedParameters);
            Assert.Contains("_summary", request.AllowedParameters);
        }
    }
}
