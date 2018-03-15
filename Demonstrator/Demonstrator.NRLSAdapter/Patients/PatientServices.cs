using Microsoft.Extensions.Options;
using SystemTasks = System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using Hl7.Fhir.Model;
using Demonstrator.Core.Interfaces.Services.Fhir;
using Demonstrator.Models.Core.Models;
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

        public async SystemTasks.Task<Bundle> GetPatientAsBundle(int nhsNumber, bool includes)
        {
            var patient = await new FhirConnector().RequestOne<Bundle>(GetPatientNhsNumberUrl(nhsNumber, includes));

            return patient;
        }

        public async SystemTasks.Task<List<Patient>> GetPatients()
        {
            var patients = await new FhirConnector().RequestMany<Patient>(GetPatientUrl());

            return patients;
        }

        private string GetPatientNhsNumberUrl(int nhsNumber, bool includes)
        {
            var parameters = $"identifier={WebUtility.UrlEncode(FhirConstants.SystemNhsNumber)}|{nhsNumber}";

            return GetPatientUrl(null, parameters, includes);
        }

        private string GetPatientUrl(string id = null, string parameters = null, bool includes = false)
        {
            if (includes)
            {
                parameters = $"{(parameters + "&" ?? "")}_include=Patient:organization";
            }

            return $"{_patientUrlBase}{("/" + id ?? "")}?_format=json{("&" + parameters ?? "")}";
        }

    }
}
