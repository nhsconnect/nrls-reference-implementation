using Demonstrator.Core.Interfaces.Services.Epr;
using Demonstrator.Models.ViewModels.Patients;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Demonstrator.WebApp.Controllers
{
    [Route("api/[controller]")]
    public class PatientsController : Controller
    {
        private readonly IPatientViewService _patientViewServices;

        public PatientsController(IPatientViewService patientViewServices)
        {
            _patientViewServices = patientViewServices;
        }

        // GET api/Patients/Numbers
        [HttpGet("Numbers")]
        public async Task<IActionResult> Get()
        {

            //Service to get Patient Numbers
            var patientNumbers = await _patientViewServices.GetPatientNumbers();

            return Ok(patientNumbers);
        }

        // GET api/Patients/1234
        [HttpGet("{nhsNumber:regex(^[[0-9]]{{10}}$)}")]
        public async Task<IActionResult> GetByNhsNumber(string nhsNumber)
        {
            //Service to get Patient by nhs number
            var patient = await _patientViewServices.GetPatient(nhsNumber);

            return Ok(patient);
        }

    }
}
