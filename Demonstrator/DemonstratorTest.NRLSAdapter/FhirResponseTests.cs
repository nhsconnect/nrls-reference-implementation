using Demonstrator.NRLSAdapter.Helpers;
using Demonstrator.NRLSAdapter.Helpers.Exceptions;
using DemonstratorTest.Comparer;
using DemonstratorTest.Data.Helpers;
using Hl7.Fhir.Model;
using System.Linq;
using Xunit;

namespace DemonstratorTest.NRLSAdapter
{
    public class FhirResponseTests
    {
        [Fact]
        public void FhirResponse_ValidPatientResources()
        {
            var patientModels = MongoPatients.Patients.ToList();
            var patientBundle = FhirBundle.GetBundle<Patient>(patientModels);

            var actual = new FhirResponse();
            actual.Resource = patientBundle;

            Assert.Equal(patientModels, actual.GetResources<Patient>(), Comparers.ModelComparer<Patient>());
        }

        [Fact]
        public void FhirResponse_ValidOrganizationResources()
        {
            var organizationModels = MongoOrganizations.Organizations.ToList();
            var organizationBundle = FhirBundle.GetBundle<Organization>(organizationModels);

            var actual = new FhirResponse();
            actual.Resource = organizationBundle;

            Assert.Equal(organizationModels, actual.GetResources<Organization>(), Comparers.ModelComparer<Organization>());
        }

        [Fact]
        public void FhirResponse_ValidDocumentReferenceResources()
        {
            var documentReferenceModels = MongoDocumentReferences.DocumentReferences.ToList();
            var documentReferenceBundle = FhirBundle.GetBundle<DocumentReference>(documentReferenceModels);

            var actual = new FhirResponse();
            actual.Resource = documentReferenceBundle;

            Assert.Equal(documentReferenceModels, actual.GetResources<DocumentReference>(), Comparers.ModelComparer<DocumentReference>());
        }

        [Fact]
        public void FhirResponse_InvalidResources()
        {
            var practitionersModels = MongoPractitioner.Practitioners.ToList();
            var practitionersBundle = FhirBundle.GetBundle<Practitioner>(practitionersModels);

            Assert.Throws<InvalidResourceException>(() => 
            {
                var actual = new FhirResponse();
                actual.Resource = practitionersBundle;

                actual.GetResources<Practitioner>();
            });
        }

    }
}
