using Hl7.Fhir.Model;
using System.Collections.Generic;
using SystemTasks = System.Threading.Tasks;

namespace Demonstrator.Core.Interfaces.Services.Fhir
{
    public interface IPatientServices
    {
        SystemTasks.Task<List<Patient>> GetPatients();

        SystemTasks.Task<Bundle> GetPatientAsBundle(string nhsNumber);
    }
}
