using Hl7.Fhir.Model;
using System.Collections.Generic;
using SystemTasks = System.Threading.Tasks;

namespace Demonstrator.Core.Interfaces.Services.Fhir
{
    public interface IOrganisationServices
    {

        SystemTasks.Task<Organization> GetOrganisation(string orgCode);

        SystemTasks.Task<List<Organization>> GetOrganisations();
    }
}
