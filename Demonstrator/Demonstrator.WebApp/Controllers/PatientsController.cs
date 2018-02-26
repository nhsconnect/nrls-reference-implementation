using Demonstrator.Core.Interfaces.Services.Fhir;
using Demonstrator.Models.ViewModels.Patients;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;

namespace Demonstrator.WebApp.Controllers
{
    [Route("api/[controller]")]
    public class PatientsController : Controller
    {
        private readonly IPatientServices _patientServices;
        public PatientsController(IPatientServices patientServices)
        {
            _patientServices = patientServices;
        }

        // GET api/Patients/Numbers
        [HttpGet("Numbers")]
        public async Task<IActionResult> Get()
        {

            //Service to get Patient Numbers
            var patientNumbers = await _patientServices.GetPatientNumbers();

            return Ok(patientNumbers);
        }

        // GET api/Patients/1234
        [HttpGet("{nhsNumber:int}")]
        public async Task<IActionResult> GetByNhsNumber(int nhsNumber)
        {
            //Service to get Patient by logical id
            var patient = await _patientServices.GetPatient(nhsNumber);

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
