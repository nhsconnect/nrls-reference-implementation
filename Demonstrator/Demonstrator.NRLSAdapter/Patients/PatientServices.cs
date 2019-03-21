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
using Microsoft.Net.Http.Headers;

namespace Demonstrator.NRLSAdapter.Patients
{
    public class PatientServices : IPatientServices
    {
        private readonly ExternalApiSetting _pdsSettings;

        public PatientServices(IOptions<ExternalApiSetting> externalApiSetting)
        {
            _pdsSettings = externalApiSetting.Value;
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
                BaseUrl = $"{(_pdsSettings.PdsUseSecure ? _pdsSettings.PdsSecureServerUrl : _pdsSettings.PdsServerUrl)}",
                ResourceType = ResourceType.Patient,
                Method = HttpMethod.Get,
                UseSecure = _pdsSettings.PdsUseSecure,
            };

            command.Headers.Add(HeaderNames.Accept, ContentType.JSON_CONTENT_HEADER);

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


    }
}
