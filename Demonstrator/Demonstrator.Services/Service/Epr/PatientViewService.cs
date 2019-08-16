using Demonstrator.Core.Interfaces.Services.Fhir;
using Demonstrator.Core.Interfaces.Services.Epr;
using Demonstrator.Models.ViewModels.Factories;
using Demonstrator.Models.ViewModels.Fhir;
using Demonstrator.Models.ViewModels.Patients;
using Demonstrator.NRLSAdapter.Helpers;
using Demonstrator.Services.Service.Base;
using Hl7.Fhir.Model;
using System.Collections.Generic;
using System.Linq;
using SystemTasks = System.Threading.Tasks;
using Demonstrator.Core.Resources;

namespace Demonstrator.Services.Service.Epr
{
    public class PatientViewService: BaseFhirService, IPatientViewService
    {
        private readonly IPatientServices _patientService;

        public PatientViewService(IPatientServices patientService)
        {
            _patientService = patientService;
        }


        public async SystemTasks.Task<IEnumerable<PatientNumberViewModel>> GetPatientNumbers()
        {
            var patients = await _patientService.GetPatients();

            var patientNumbers = new List<PatientNumberViewModel>();

            patients.ForEach(p => {
                var nhsNumber = p.Identifier.FirstOrDefault(i => i.System.Equals(FhirConstants.SystemNhsNumber));

                var patientNumber = new PatientNumberViewModel
                {
                    Id = p.Id,
                    NhsNumber = nhsNumber?.Value
                };

                patientNumbers.Add(patientNumber);
            });

            return await SystemTasks.Task.Run(() => patientNumbers);
        }

        public async SystemTasks.Task<PatientViewModel> GetPatient(string nhsNumber)
        {
            var patientViewModel = new PatientViewModel();

            var bundle = await _patientService.GetPatientAsBundle(nhsNumber);
            var entries = bundle.Entry;

            var patient = ListEntries<Patient>(entries, ResourceType.Patient).FirstOrDefault();
            var organisations = ListEntries<Organization>(entries, ResourceType.Organization);

            if(patient != null)
            {
                patientViewModel = patient.ToViewModel(FhirConstants.SystemNhsNumber);

                var gpPractice = organisations.FirstOrDefault(s => !string.IsNullOrWhiteSpace(s.Id) && s.Id == patientViewModel.ManagingOrganization?.Id);
                patientViewModel.GpPractice = gpPractice?.ToViewModel(FhirConstants.SystemOrgCode);
            }

            return patientViewModel;

        }


    }
}
