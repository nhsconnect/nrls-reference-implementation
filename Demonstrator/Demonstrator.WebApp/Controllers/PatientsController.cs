using Demonstrator.Core.Interfaces.Services.Nrls;
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
        [HttpGet("{nhsNumber:regex(^[[0-9]]{{9}}$)}")]
        public async Task<IActionResult> GetByNhsNumber(string nhsNumber)
        {
            //Service to get Patient by nhs number
            var patient = await _patientViewServices.GetPatient(nhsNumber);

            return Ok(patient);
        }

        // POST api/Patients/1234/Records
        [HttpPost("{patientId:int}/Records")]
        public IActionResult Create([FromRoute] int patientId, [FromBody] MedicalRecord medicalRecord)
        {

            //Service to create new Patient record

            return Ok($"Create patient record for patient id {patientId}");
        }

        // PUT api/Patients/1234/Records/4567
        [HttpPut("{patientId:int}/Records/{recordId:int}")]
        public IActionResult Update([FromRoute] int patientId, [FromRoute] int recordId, [FromBody] MedicalRecord medicalRecord)
        {

            //Service to update a Patient record

            return Ok($"Update patient record of id {recordId} for patient id {patientId}");
        }

    }
}
