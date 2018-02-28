using Microsoft.Extensions.Options;
using SystemTasks = System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Hl7.Fhir.Model;
using Demonstrator.Core.Interfaces.Services.Fhir;
using Demonstrator.Models.Core.Models;
using Demonstrator.Models.ViewModels.Patients;
using Demonstrator.NRLSAdapter.Helpers;

namespace Demonstrator.NRLSAdapter.Patients
{
    public class PatientServices : IPatientServices
    {
        private string _patientUrlBase;

        public PatientServices(IOptions<NrlsApiSetting> nrlsApiSetting)
        {
            _patientUrlBase = $"{nrlsApiSetting.Value.ServerUrl}/Patient";
        }

        public async SystemTasks.Task<IEnumerable<PatientNumberViewModel>> GetPatientNumbers()
        {
            var patients = GetPatients().Result;
            var patientNumbers = new List<PatientNumberViewModel>();

            patients.ForEach(p => {
                var nhsNumber = p.Identifier.FirstOrDefault(i => i.System.Equals(FhirConstants.SystemNhsNumber));

                var patientNumber = new PatientNumberViewModel {
                    Id = p.Id,
                    NhsNumber = !string.IsNullOrEmpty(nhsNumber?.Value) ? int.Parse(nhsNumber.Value) : (int?)null
                };

                patientNumbers.Add(patientNumber);
            });

            return await SystemTasks.Task.Run(() => patientNumbers);
        }

        public async SystemTasks.Task<Patient> GetPatient(int nhsNumber)
        {
            var patients = await new FhirConnector().RequestMany<Patient>(GetPatientNhsNumberUrl(nhsNumber));

            return patients.First();
        }

        private async SystemTasks.Task<List<Patient>> GetPatients()
        {
            var patients = await new FhirConnector().RequestMany<Patient>(GetPatientUrl());

            return patients;
        }

        private string GetPatientNhsNumberUrl(int nhsNumber)
        {
            var parameters = $"identifier={WebUtility.UrlEncode(FhirConstants.SystemNhsNumber)}|{nhsNumber}";

            return GetPatientUrl(null, parameters);
        }

        private string GetPatientUrl(string id = null, string parameters = null)
        {
            return $"{_patientUrlBase}{("/" + id ?? "")}?_format=json{("&" + parameters ?? "")}";
        }

    }
}
