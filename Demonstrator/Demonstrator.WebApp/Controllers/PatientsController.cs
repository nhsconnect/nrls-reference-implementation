using Demonstrator.Models.ViewModels.Patients;
using Microsoft.AspNetCore.Mvc;

namespace Demonstrator.WebApp.Controllers
{
    [Route("api/[controller]")]
    public class PatientsController : Controller
    {
        // GET api/Patients/Numbers
        [HttpGet("Numbers")]
        public IActionResult Get()
        {

            //Service to get Patient Numbers

            return Ok($"Get all Patient Numbers.");
        }

        // GET api/Patients/1234
        [HttpGet("{patientId:int}")]
        public IActionResult Get(int patientId)
        {

            //Service to get Patient by logical id

            return Ok($"Get patient with id {patientId}");
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
        public IActionResult Create([FromRoute] int patientId, [FromRoute] int recordId, [FromBody] MedicalRecord medicalRecord)
        {

            //Service to update a Patient record

            return Ok($"Update patient record of id {recordId} for patient id {patientId}");
        }

    }
}
