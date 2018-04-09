using Demonstrator.Core.Interfaces.Services.Fhir;
using Demonstrator.Core.Interfaces.Services.Nrls;
using Demonstrator.Models.Nrls;
using Demonstrator.Models.ViewModels.Base;
using Demonstrator.Models.ViewModels.Factories;
using Demonstrator.Models.ViewModels.Nrls;
using Demonstrator.NRLSAdapter.Helpers;
using Demonstrator.Services.Service.Base;
using Hl7.Fhir.Model;
using System.Collections.Generic;
using System.Linq;
using SystemTasks = System.Threading.Tasks;

namespace Demonstrator.Services.Service.Nrls
{
    public class PointerService : BaseFhirService, IPointerService
    {
        private readonly IDocumentReferenceServices _docRefService;
        private readonly IPatientServices _patientService;
        private readonly IOrganisationServices _organisationServices;

        public PointerService(IDocumentReferenceServices docRefService, IPatientServices patientService, IOrganisationServices organisationServices)
        {
            _docRefService = docRefService;
            _patientService = patientService;
            _organisationServices = organisationServices;
        }

        public async SystemTasks.Task<IEnumerable<PointerViewModel>> GetPointers(RequestViewModel request)
        {
            var pointerViewModels = new List<PointerViewModel>();

            var pointerRequest = NrlsPointerRequest.Search(null, request.Id, request.Asid, FhirConstants.SearchInteractionId);

            var pointerBundle = await _docRefService.GetPointersAsBundle(pointerRequest);
            var pointerEntries = pointerBundle.Entry;

            //need a more slick solution for getting related references
            //we are connecting to NRLS so will only get Pointers back - a complete Fhir server would allow for includes
            var patients = await _patientService.GetPatients(); //In live this could be lots
            var organisations = await _organisationServices.GetOrganisations(); //In live this could be lots

            var pointers = ListEntries<DocumentReference>(pointerEntries, ResourceType.DocumentReference);
            //var patients = ListEntries<Patient>(entries, ResourceType.Patient); // If we could do includes take from bundle
            //var organisations = ListEntries<Organization>(entries, ResourceType.Organization); // If we could do includes take from bundle

            foreach (var pointer in pointers)
            {
                var pointerViewModel = pointer.ToViewModel();
                var patientNhsNumber = pointerViewModel.Subject?.Reference?.Replace(FhirConstants.SystemPDS, "");
                var authorOrgCode = pointerViewModel.Author?.Reference?.Replace(FhirConstants.SystemODS, "");
                var custodianOrgCode = pointerViewModel.Custodian?.Reference?.Replace(FhirConstants.SystemODS, "");

                //This assumes the resource is relative
                //In reality it does not make sense to attach a patient because a GET to NRLS should be in the patient context anyway!
                var subject = patients.FirstOrDefault(s => s.Identifier.FirstOrDefault(t => !string.IsNullOrEmpty(patientNhsNumber) && !string.IsNullOrEmpty(t.System) && t.System.Equals(FhirConstants.IdsNhsNumber) && !string.IsNullOrEmpty(t.Value) && t.Value.Equals(patientNhsNumber)) != null);
                pointerViewModel.SubjectViewModel = subject?.ToViewModel(null);

                //This assumes the resource is relative
                var custodian = organisations.FirstOrDefault(s => s.Identifier.FirstOrDefault(t => !string.IsNullOrEmpty(custodianOrgCode) && !string.IsNullOrEmpty(t.System) && t.System.Equals(FhirConstants.IdsOrgCode) && !string.IsNullOrEmpty(t.Value) && t.Value.Equals(custodianOrgCode)) != null);
                pointerViewModel.CustodianViewModel = custodian?.ToViewModel(FhirConstants.IdsOrgCode);

                var author = organisations.FirstOrDefault(s => s.Identifier.FirstOrDefault(t => !string.IsNullOrEmpty(authorOrgCode) && !string.IsNullOrEmpty(t.System) && t.System.Equals(FhirConstants.IdsOrgCode) && !string.IsNullOrEmpty(t.Value) && t.Value.Equals(authorOrgCode)) != null);
                pointerViewModel.AuthorViewModel = author?.ToViewModel(FhirConstants.IdsOrgCode);

                pointerViewModels.Add(pointerViewModel);
            }

            return pointerViewModels;
        }

    }
}
