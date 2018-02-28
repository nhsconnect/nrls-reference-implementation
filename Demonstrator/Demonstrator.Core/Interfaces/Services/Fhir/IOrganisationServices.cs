using Hl7.Fhir.Model;
using SystemTasks = System.Threading.Tasks;

namespace Demonstrator.Core.Interfaces.Services.Fhir
{
    public interface IOrganisationServices
    {

        SystemTasks.Task<Organization> GetOrganisation(string orgCode);
    }
}
