using Microsoft.Extensions.Options;
using SystemTasks = System.Threading.Tasks;
using System.Collections.Generic;
using System.Net;
using Hl7.Fhir.Model;
using Demonstrator.Core.Interfaces.Services.Fhir;
using Demonstrator.Models.Core.Models;
using Demonstrator.NRLSAdapter.Helpers;
using Hl7.Fhir.Rest;
using Demonstrator.NRLSAdapter.Models;
using System.Net.Http;

namespace Demonstrator.NRLSAdapter.Patients
{
    public class PatientServices : IPatientServices
    {
        private string _patientUrlBase;

        public PatientServices(IOptions<ExternalApiSetting> externalApiSetting)
        {
            _patientUrlBase = $"{externalApiSetting.Value.PdsServerUrl}";
        }

        public async SystemTasks.Task<Bundle> GetPatientAsBundle(string nhsNumber)
        {
            var patient = await new FhirConnector().RequestOne<Bundle>(BuildRequest(nhsNumber));

            return patient;
        }


        public async SystemTasks.Task<List<Patient>> GetPatients()
        {
            var patients = await new FhirConnector().RequestMany<Patient>(BuildRequest(null));

            return patients;
        }

        private CommandRequest BuildRequest(string nhsNumber)
        {
            var command = new CommandRequest
            {
                BaseUrl = _patientUrlBase,
                ResourceType = ResourceType.Patient,
                Method = HttpMethod.Get
            };

            if (!string.IsNullOrEmpty(nhsNumber))
            {
                command.SearchParams = GetParams(nhsNumber);
            }

            return command;
        }

        private SearchParams GetParams(string nhsNumber)
        {
            var searchParams = new SearchParams();
            searchParams.Add("identifier", $"{WebUtility.UrlEncode(FhirConstants.IdsNhsNumber)}|{nhsNumber}");

            return searchParams;
        }

        //private string GetPatientNhsNumberUrl(string nhsNumber, bool includes)
        //{
        //    var parameters = $"identifier={WebUtility.UrlEncode(FhirConstants.SystemNhsNumber)}|{nhsNumber}";

        //    return GetPatientUrl(null, parameters, includes);
        //}

        //private string GetPatientUrl(string id = null, string parameters = null, bool includes = false)
        //{
        //    if (includes)
        //    {
        //        parameters = $"{(parameters + "&" ?? "")}_include=Patient:organization";
        //    }

        //    return $"{_patientUrlBase}{("/" + id ?? "")}?_format=json{("&" + parameters ?? "")}";
        //}

    }
}
