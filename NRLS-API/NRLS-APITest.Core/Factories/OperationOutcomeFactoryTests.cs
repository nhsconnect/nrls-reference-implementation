using Hl7.Fhir.Model;
using NRLS_API.Core.Factories;
using NRLS_APITest.Comparer;
using NRLS_APITest.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Xunit;

namespace NRLS_APITest.Core.Factories
{
    public class OperationOutcomeFactoryTests
    {
        [Fact]
        public void CreateDelete_Is_Valid()
        {
            var expected = OperationOutcomes.Deleted;

            var actual = OperationOutcomeFactory.CreateDelete("https://testdomain/testurl", "91370360-d667-4bc8-bebe-f223560ff90e");

            Assert.Equal(expected, actual, Comparers.ModelComparer<OperationOutcome>());
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

            var expected = new OperationOutcome()
            {
                //Meta = new Meta
                //{
                //    Profile = new List<string>{
                //        "https://fhir.nhs.uk/STU3/StructureDefinition/Spine-OperationOutcome-1-0"
                //    }
                //},
                Issue = new List<OperationOutcome.IssueComponent>
                {
                    new OperationOutcome.IssueComponent
                    {
                        Severity = OperationOutcome.IssueSeverity.Error,
                        Code = OperationOutcome.IssueType.NotFound,
                        Diagnostics = "The ODS code in the custodian and/or author element is not resolvable – testid.",
                        Details = new CodeableConcept
                        {
                            Text = null,
                            Coding = new List<Coding>
                            {
                                new Coding
                                {
                                    System = "https://fhir.nhs.uk/STU3/ValueSet/spine-response-code-2-0",
                                    Code = "ORGANISATION_NOT_FOUND",
                                    Display = "Organisation record not found"
                                }
                            }
                        }
                    }
                }
            };

            var actual = OperationOutcomeFactory.CreateOrganizationNotFound("testid");

            Assert.Equal(expected, actual, Comparers.ModelComparer<OperationOutcome>());
        }
    }
}
