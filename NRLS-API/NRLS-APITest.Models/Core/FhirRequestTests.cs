using NRLS_APITest.Data;
using System;
using Xunit;

namespace NRLS_APITest.Models
{
    public class FhirRequestTests
    {
        [Fact]
        public void FhirRequest_ValidIdParam()
        {
            var expected = "testId";

            var request = FhirRequests.Valid_Delete;

            Assert.Equal(expected, request.IdParameter);
        }

        [Fact]
        public void FhirRequest_ValidHasIdParam()
        {
            var request = FhirRequests.Valid_Delete;

            Assert.True(request.HasIdParameter);
        }

        [Fact]
        public void FhirRequest_ValidHasNoIdParam()
        {
            var request = FhirRequests.Invalid_ConditionalDelete_NoSearchValues;

            Assert.False(request.HasIdParameter);
        }

        [Fact]
        public void FhirRequest_ValidIdentifierParam()
        {
            var expected = "testsystem|testvalue";

            var request = FhirRequests.Valid_ConditionalDelete;

            Assert.Equal(expected, request.IdentifierParameter);
        }

        [Fact]
        public void FhirRequest_ValidSubjectParam()
        {
            var expected = "https://demographics.spineservices.nhs.uk/STU3/Patient/2686033207";

            var request = FhirRequests.Valid_ConditionalDelete;

            Assert.Equal(expected, request.SubjectParameter);
        }
    }
}
