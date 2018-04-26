using Demonstrator.Core.Interfaces.Services.Fhir;
using Demonstrator.Models.Nrls;
using DemonstratorTest.Data.Helpers;
using Hl7.Fhir.Model;
using Moq;
using System.Collections.Generic;
using System.Linq;
using SystemTasks = System.Threading.Tasks;
using Xunit;
using Demonstrator.Services.Service.Nrls;
using Demonstrator.Models.ViewModels.Base;
using DemonstratorTest.Comparer;
using Demonstrator.Models.ViewModels.Nrls;

namespace DemonstratorTest.Services.Nrls
{
    public class PointerServiceTests
    {
        [Fact]
        public async void PointerService_Returns_ValidPointerList()
        {
            var patientModels = MongoPatients.Patients.ToList();
            var organizationModels = MongoOrganizations.Organizations.ToList();
            var documentReferenceModels = MongoDocumentReferences.DocumentReferences_3656987882;

             var testNhsNumber = "3656987882";
            var testAsid = "200000000116";
            var testOrgCode = "AMS01";

            var _docRefService = new Mock<IDocumentReferenceServices>();
            _docRefService.Setup(m => m.GetPointersAsBundle(It.IsAny<NrlsPointerRequest>())).Returns(SystemTasks.Task.Run(() => FhirBundle.GetBundle<DocumentReference>(documentReferenceModels))).Verifiable();

            var _patientService = new Mock<IPatientServices>();
            _patientService.Setup(m => m.GetPatients()).Returns(SystemTasks.Task.Run(() => patientModels)).Verifiable();

            var _organisationServices = new Mock<IOrganisationServices>();
            _organisationServices.Setup(m => m.GetOrganisations()).Returns(SystemTasks.Task.Run(() => organizationModels)).Verifiable();

            var pointerService = new PointerService(_docRefService.Object, _patientService.Object, _organisationServices.Object);

            var request = RequestViewModel.Create(testNhsNumber);
            request.Asid = testAsid;
            request.OrgCode = testOrgCode;

            var actual = await pointerService.GetPointers(request);

            var pointers = MongoPointerViewModels.PointerViewModels_3656987882;
            var patient = MongoPatientViewModels.PatientViewModel_3656987882;
            var org = MongoOrganizationViewModels.OrganizationViewModel_00003X;

            var expected = new List<PointerViewModel>();

            foreach (var exp in pointers)
            {
                exp.SubjectViewModel = patient;
                exp.AuthorViewModel = org;
                exp.CustodianViewModel = org;
                expected.Add(exp);
            }

            _docRefService.Verify();
            _patientService.Verify();
            _organisationServices.Verify();

            Assert.Equal(expected, actual, Comparers.ModelComparer<PointerViewModel>());

        }

    }
}
