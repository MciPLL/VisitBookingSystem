using System.Text;
using Microsoft.AspNetCore.Mvc;
using VisitBookingSystem.DTOs;
using VisitBookingSystem.Services;

namespace VisitBookingSystem.Controllers
{
    /// <summary>
    /// Zarządzanie pacjentami.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly IPatientService _patientService;

        public PatientsController(IPatientService patientService)
        {
            _patientService = patientService;
        }

        /// <summary>Pobiera listę wszystkich pacjentów z opcjonalnym sortowaniem.</summary>
        /// <param name="sortBy">Pole sortowania: <c>lastName</c> (domyślnie), <c>firstName</c>, <c>email</c>.</param>
        /// <param name="sortOrder">Kierunek sortowania: <c>asc</c> (domyślnie) lub <c>desc</c>.</param>
        /// <returns>Lista pacjentów.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<PatientDto>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<PatientDto>> GetPatients([FromQuery] string? sortBy = null, [FromQuery] string? sortOrder = null)
        {
            return Ok(_patientService.GetAll(sortBy, sortOrder));
        }

        /// <summary>Pobiera pacjenta o podanym identyfikatorze.</summary>
        /// <param name="id">Identyfikator pacjenta.</param>
        /// <returns>Dane pacjenta.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(PatientDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<PatientDto> GetPatient(int id)
        {
            return Ok(_patientService.GetById(id));
        }

        /// <summary>Tworzy nowego pacjenta.</summary>
        /// <param name="createPatientDto">Dane nowego pacjenta.</param>
        /// <returns>Utworzony pacjent.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(PatientDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public ActionResult<PatientDto> CreatePatient(CreatePatientDto createPatientDto)
        {
            var patient = _patientService.Create(createPatientDto);
            return CreatedAtAction(nameof(GetPatient), new { id = patient.Id }, patient);
        }

        /// <summary>Aktualizuje dane istniejącego pacjenta.</summary>
        /// <param name="id">Identyfikator pacjenta.</param>
        /// <param name="updatePatientDto">Nowe dane pacjenta.</param>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdatePatient(int id, CreatePatientDto updatePatientDto)
        {
            _patientService.Update(id, updatePatientDto);
            return NoContent();
        }

        /// <summary>Trwale usuwa pacjenta z bazy danych. Powiązane wizyty również zostają usunięte.</summary>
        /// <param name="id">Identyfikator pacjenta.</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeletePatient(int id)
        {
            _patientService.Delete(id);
            return NoContent();
        }

        /// <summary>Eksportuje listę pacjentów do pliku CSV.</summary>
        /// <returns>Plik CSV z danymi pacjentów.</returns>
        [HttpGet("export")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        public IActionResult ExportPatients()
        {
            var csv = _patientService.ExportToCsv();
            var bytes = new System.Text.UTF8Encoding(true).GetBytes(csv);
            return File(bytes, "text/csv; charset=utf-8", "patients.csv");
        }
    }
}
