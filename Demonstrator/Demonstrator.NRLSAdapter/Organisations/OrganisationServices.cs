using Demonstrator.Models.Core.Models;
using Demonstrator.Core.Interfaces.Services.Fhir;
using Demonstrator.NRLSAdapter.Helpers;
using Hl7.Fhir.Model;
using Microsoft.Extensions.Options;
using SystemTasks = System.Threading.Tasks;
using System.Net;
using System.Linq;


namespace Demonstrator.NRLSAdapter.Organisations
{
    public class OrganisationServices : IOrganisationServices
    {
        private string _organisationUrlBase;

        public OrganisationServices(IOptions<NrlsApiSetting> nrlsApiSetting)
        {
            _organisationUrlBase = $"{nrlsApiSetting.Value.ServerUrl}/Organization";
        }

        public async SystemTasks.Task<Organization> GetOrganisation(string orgCode)
        {
            var orgs = await new FhirConnector().RequestMany<Organization>(GetOrgCodeUrl(orgCode));

            return orgs.First();
        }

        private string GetOrgCodeUrl(string orgCode)
        {
            var parameters = $"identifier={WebUtility.UrlEncode(FhirConstants.SystemOrgCode)}|{orgCode}";

            return GetOrganisationUrl(null, parameters);
        }

        private string GetOrganisationUrl(string id = null, string parameters = null)
        {
            return $"{_organisationUrlBase}{("/" + id ?? "")}?_format=json{("&" + parameters ?? "")}";
        }
    }
}
