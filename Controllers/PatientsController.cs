using Microsoft.AspNetCore.Mvc;
using VisitBookingSystem.DTOs;
using VisitBookingSystem.Services;

namespace VisitBookingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<PatientDto>> GetPatients()
        {
            return Ok(_patientService.GetAll());
        }

        [HttpGet("{id}")]
        public ActionResult<PatientDto> GetPatient(int id)
        {
            return Ok(_patientService.GetById(id));
        }

        [HttpPost]
        public ActionResult<PatientDto> CreatePatient(CreatePatientDto createPatientDto)
        {
            var patient = _patientService.Create(createPatientDto);
            return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
        }

        [HttpPut("{id}")]
        public IActionResult UpdatePatient(int id, CreatePatientDto updatePatientDto)
        {
            _patientService.Update(id, updatePatientDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeletePatient(int id)
        {
            _patientService.Delete(id);
            return NoContent();
        }
    }
}