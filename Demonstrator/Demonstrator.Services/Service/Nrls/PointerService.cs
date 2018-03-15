using Demonstrator.Core.Interfaces.Services.Fhir;
using Demonstrator.Core.Interfaces.Services.Nrls;
using Demonstrator.Models.ViewModels.Factories;
using Demonstrator.Models.ViewModels.Nrls;
using Demonstrator.NRLSAdapter.Helpers;
using Hl7.Fhir.Model;
using System.Collections.Generic;
using System.Linq;
using SystemTasks = System.Threading.Tasks;

namespace Demonstrator.Services.Service.Nrls
{
    public class PointerService : BaseService, IPointerService
    {
        private readonly IDocumentReferenceServices _docRefService;

        public PointerService(IDocumentReferenceServices docRefService)
        {
            _docRefService = docRefService;
        }

        public async SystemTasks.Task<IEnumerable<PointerViewModel>> GetPointers(int? nhsNumber, string orgCode)
        {
            var pointerViewModels = new List<PointerViewModel>();

            var bundle = await _docRefService.GetPointersAsBundle(true, nhsNumber, orgCode);
            var entries = bundle.Entry;

            var pointers = ListEntries<DocumentReference>(entries, ResourceType.DocumentReference);
            var patients = ListEntries<Patient>(entries, ResourceType.Patient);
            var organisations = ListEntries<Organization>(entries, ResourceType.Organization);

            foreach(var pointer in pointers)
            {
                var pointerViewModel = pointer.ToViewModel();

                //This assumes the resource is relative
                var subject = patients.FirstOrDefault(s => !string.IsNullOrWhiteSpace(s.Id) && s.Id == pointerViewModel.Subject?.Id);
                pointerViewModel.SubjectViewModel = subject?.ToViewModel(null);

                //This assumes the resource is relative
                var custodian = organisations.FirstOrDefault(s => !string.IsNullOrWhiteSpace(s.Id) && s.Id == pointerViewModel.Custodian?.Id);
                pointerViewModel.CustodianViewModel = custodian?.ToViewModel(FhirConstants.SystemOrgCode);

                var author = organisations.FirstOrDefault(s => !string.IsNullOrWhiteSpace(s.Id) && s.Id == pointerViewModel.Author?.Id);
                pointerViewModel.AuthorViewModel = author?.ToViewModel(FhirConstants.SystemOrgCode);

                pointerViewModels.Add(pointerViewModel);
            }

            return pointerViewModels;
        }

    }
}
