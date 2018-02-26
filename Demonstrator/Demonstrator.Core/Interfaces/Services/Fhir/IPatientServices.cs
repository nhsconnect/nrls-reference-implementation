using Demonstrator.Models.ViewModels.Patients;
using Hl7.Fhir.Model;
using System.Collections.Generic;
using SystemTasks = System.Threading.Tasks;

namespace Demonstrator.Core.Interfaces.Services.Fhir
{
    public interface IPatientServices
    {
        SystemTasks.Task<IEnumerable<PatientNumberViewModel>> GetPatientNumbers();

        SystemTasks.Task<Patient> GetPatient(int nhsNumber);
    }
}
