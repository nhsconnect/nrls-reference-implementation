using Demonstrator.Models.ViewModels.Fhir;
using Demonstrator.Models.ViewModels.Patients;
using System.Collections.Generic;
using SystemTasks = System.Threading.Tasks;

namespace Demonstrator.Core.Interfaces.Services.Epr
{
    public interface IPatientViewService
    {
        SystemTasks.Task<IEnumerable<PatientNumberViewModel>> GetPatientNumbers();

        SystemTasks.Task<PatientViewModel> GetPatient(string nhsNumber);
    }
}
