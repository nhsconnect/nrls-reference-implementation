using Demonstrator.Core.Factories;
using Hl7.Fhir.Model;
using System.Linq;
using Xunit;

namespace DemonstratorTest.Core.Factories
{
    public class OperationOutcomeFactoryTests
    {
        [Fact]
        public void CreateDelete_Is_Valid()
        {
            var actual = OperationOutcomeFactory.CreateDelete("https://testdomain/testurl", "91370360-d667-4bc8-bebe-f223560ff90e");

            Assert.IsType<OperationOutcome>(actual);

            Assert.NotNull(actual.Issue);
            Assert.NotEmpty(actual.Issue);

            Assert.Equal("Successfully removed resource DocumentReference: https://testdomain/testurl", actual.Issue.First().Diagnostics);
            Assert.Equal(OperationOutcome.IssueType.Informational.ToString(), actual.Issue.First().Code.ToString());
            Assert.Equal(OperationOutcome.IssueSeverity.Information.ToString(), actual.Issue.First().Severity.ToString());

            Assert.NotNull(actual.Issue.First().Details);
            Assert.NotNull(actual.Issue.First().Details.Coding);
            Assert.NotEmpty(actual.Issue.First().Details.Coding);

            Assert.Equal("91370360-d667-4bc8-bebe-f223560ff90e", actual.Issue.First().Details.Text);

            Assert.Equal("RESOURCE_DELETED", actual.Issue.First().Details.Coding.First().Code);
            Assert.Equal("Resource removed", actual.Issue.First().Details.Coding.First().Display);

        }

        [Fact]
        public void CreateDuplicate_Is_Valid()
        {
            var identifier = new Identifier("testsystem", "testvalue");
            var actual = OperationOutcomeFactory.CreateDuplicateRequest(identifier);

            Assert.IsType<OperationOutcome>(actual);

            Assert.NotNull(actual.Issue);
            Assert.NotEmpty(actual.Issue);

            Assert.Equal("Duplicate masterIdentifier value: testvalue system: testsystem", actual.Issue.First().Diagnostics);
            Assert.Equal(OperationOutcome.IssueType.Duplicate.ToString(), actual.Issue.First().Code.ToString());
            Assert.Equal(OperationOutcome.IssueSeverity.Error.ToString(), actual.Issue.First().Severity.ToString());

            Assert.NotNull(actual.Issue.First().Details);
            Assert.NotNull(actual.Issue.First().Details.Coding);
            Assert.NotEmpty(actual.Issue.First().Details.Coding);

            Assert.Equal("DUPLICATE_REJECTED", actual.Issue.First().Details.Coding.First().Code);
            Assert.Equal("Duplicate DocumentReference", actual.Issue.First().Details.Coding.First().Display);
        }

        //more tests required

        [Fact]
        public void CreateOrganizationNotFound_Is_Valid()
        {

            var actual = OperationOutcomeFactory.CreateOrganizationNotFound("testid");

            //Base checks for all OperactionOutcomes
            Assert.IsType<OperationOutcome>(actual);

            Assert.NotNull(actual.Issue);
            Assert.NotEmpty(actual.Issue);

            Assert.NotNull(actual.Issue.First().Details);
            Assert.NotNull(actual.Issue.First().Details.Coding);
            Assert.NotEmpty(actual.Issue.First().Details.Coding);


            //Specific OperationOutcome checks
            Assert.Equal("The ODS code in the custodian and/or author element is not resolvable – testid.", actual.Issue.First().Diagnostics);
            Assert.Equal(OperationOutcome.IssueType.NotFound.ToString(), actual.Issue.First().Code.ToString());
            Assert.Equal(OperationOutcome.IssueSeverity.Error.ToString(), actual.Issue.First().Severity.ToString());

            Assert.Equal("ORGANISATION_NOT_FOUND", actual.Issue.First().Details.Coding.First().Code);
            Assert.Equal("Organisation record not found", actual.Issue.First().Details.Coding.First().Display);
            Assert.Equal("https://fhir.nhs.uk/STU3/CodeSystem/Spine-ErrorOrWarningCode-1", actual.Issue.First().Details.Coding.First().System);
        }

        [Fact]
        public void CreateInternalError_Is_Valid()
        {

            var actual = OperationOutcomeFactory.CreateInternalError("internal error");

            //Base checks for all OperactionOutcomes
            Assert.IsType<OperationOutcome>(actual);

            Assert.NotNull(actual.Issue);
            Assert.NotEmpty(actual.Issue);

            Assert.NotNull(actual.Issue.First().Details);
            Assert.NotNull(actual.Issue.First().Details.Coding);
            Assert.NotEmpty(actual.Issue.First().Details.Coding);


            //Specific OperationOutcome checks
            Assert.Equal("internal error", actual.Issue.First().Diagnostics);
            Assert.Equal(OperationOutcome.IssueType.Invalid.ToString(), actual.Issue.First().Code.ToString());
            Assert.Equal(OperationOutcome.IssueSeverity.Error.ToString(), actual.Issue.First().Severity.ToString());

            Assert.Equal("INTERNAL_SERVER_ERROR", actual.Issue.First().Details.Coding.First().Code);
            Assert.Equal("Unexpected internal server error", actual.Issue.First().Details.Coding.First().Display);
            Assert.Equal("https://fhir.nhs.uk/STU3/CodeSystem/Spine-ErrorOrWarningCode-1", actual.Issue.First().Details.Coding.First().System);
        }

        [Fact]
        public void CreateInternalError_WithoutDiags_Is_Valid()
        {

            var actual = OperationOutcomeFactory.CreateInternalError(null);

            //Base checks for all OperactionOutcomes
            Assert.IsType<OperationOutcome>(actual);

            Assert.NotNull(actual.Issue);
            Assert.NotEmpty(actual.Issue);

            Assert.NotNull(actual.Issue.First().Details);
            Assert.NotNull(actual.Issue.First().Details.Coding);
            Assert.NotEmpty(actual.Issue.First().Details.Coding);

            //Specific OperationOutcome checks
            Assert.Null(actual.Issue.First().Diagnostics);
            Assert.Equal(OperationOutcome.IssueType.Invalid.ToString(), actual.Issue.First().Code.ToString());
            Assert.Equal(OperationOutcome.IssueSeverity.Error.ToString(), actual.Issue.First().Severity.ToString());

            Assert.Equal("INTERNAL_SERVER_ERROR", actual.Issue.First().Details.Coding.First().Code);
            Assert.Equal("Unexpected internal server error", actual.Issue.First().Details.Coding.First().Display);
            Assert.Equal("https://fhir.nhs.uk/STU3/CodeSystem/Spine-ErrorOrWarningCode-1", actual.Issue.First().Details.Coding.First().System);
        }
    }
}

