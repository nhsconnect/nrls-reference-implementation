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
using Demonstrator.Core.Resources;

namespace Demonstrator.NRLSAdapter.Patients
{
    public class PatientServices : IPatientServices
    {
        private readonly ExternalApiSetting _pdsSettings;
        private readonly IFhirConnector _fhirConnector;

        public PatientServices(IOptions<ExternalApiSetting> externalApiSetting, IFhirConnector fhirConnector)
        {
            _pdsSettings = externalApiSetting.Value;
            _fhirConnector = fhirConnector;
        }

        public async SystemTasks.Task<Bundle> GetPatientAsBundle(string nhsNumber)
        {
            var patient = await _fhirConnector.RequestOneFhir<CommandRequest, Bundle>(BuildRequest(nhsNumber));

            return patient;
        }


        public async SystemTasks.Task<List<Patient>> GetPatients()
        {
            var patients = await _fhirConnector.RequestMany<CommandRequest, Patient>(BuildRequest(null));

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
            searchParams.Add("identifier", $"{WebUtility.UrlEncode(FhirConstants.SystemNhsNumber)}|{nhsNumber}");

            return searchParams;
        }


    }
}
