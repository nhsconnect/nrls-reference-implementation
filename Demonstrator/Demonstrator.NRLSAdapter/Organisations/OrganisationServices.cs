using Demonstrator.Models.Core.Models;
using Demonstrator.Core.Interfaces.Services.Fhir;
using Demonstrator.NRLSAdapter.Helpers;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Options;
using SystemTasks = System.Threading.Tasks;
using System.Net;
using System.Linq;
using Hl7.Fhir.Rest;
using Demonstrator.NRLSAdapter.Models;
using System.Collections.Generic;
using System.Net.Http;

namespace Demonstrator.NRLSAdapter.Organisations
{
    public class OrganisationServices : IOrganisationServices
    {
        private readonly ExternalApiSetting _odsSettings;

        public OrganisationServices(IOptions<ExternalApiSetting> externalApiSetting)
        {
            _odsSettings = externalApiSetting.Value;
        }

        public async SystemTasks.Task<Organization> GetOrganisation(string orgCode)
        {
            var orgs = await new FhirConnector().RequestMany<Organization>(BuildRequest(orgCode));

            return orgs.First();
        }

        public async SystemTasks.Task<List<Organization>> GetOrganisations()
        {
            var patients = await new FhirConnector().RequestMany<Organization>(BuildRequest(null));

            return patients;
        }

        private CommandRequest BuildRequest(string orgCode)
        {
            var command = new CommandRequest
            {
                BaseUrl = $"{(_odsSettings.OdsUseSecure ? _odsSettings.OdsSecureServerUrl : _odsSettings.OdsServerUrl)}",
                ResourceType = ResourceType.Organization,
                Method = HttpMethod.Get,
                UseSecure = _odsSettings.OdsUseSecure
            };

            if (!string.IsNullOrEmpty(orgCode))
            {
                command.SearchParams = GetParams(orgCode);
            }

            return command;
        }

        private SearchParams GetParams(string orgCode)
        {
            var searchParams = new SearchParams();
            searchParams.Add("identifier", $"{WebUtility.UrlEncode(FhirConstants.IdsOrgCode)}|{orgCode}");

            return searchParams;
        }
    }
}
