using Microsoft.AspNetCore.Mvc;
using VisitBookingSystem.Data;
using VisitBookingSystem.DTOs;
using VisitBookingSystem.Models;

namespace VisitBookingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PatientsController : ControllerBase
    {
        private readonly IInMemoryDatabase _db;

        public PatientsController(IInMemoryDatabase db)
        {
            _db = db;
        }

        // pobieramy listę wszystkich pacjentów
        [HttpGet]
        public ActionResult<IEnumerable<PatientDto>> GetPatients()
        {
            return Ok(_db.Patients.Select(p => new PatientDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email
            }));
        }

        // szukamy konkretnego pacjenta po jego id
        [HttpGet("{id}")]
        public ActionResult<PatientDto> GetPatient(int id)
        {
            var patient = _db.Patients.FirstOrDefault(p => p.Id == id);

            if (patient == null)
            {
                return NotFound();
            }

            return Ok(new PatientDto
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Email = patient.Email
            });
        }

        // dodawanie nowego pacjenta do systemu
        [HttpPost]
        public ActionResult<PatientDto> CreatePatient(CreatePatientDto createPatientDto)
        {
            // sprawdzamy czy ten email nie jest już zajęty
            if (_db.Patients.Any(p => p.Email.Equals(createPatientDto.Email, StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest("Pacjent z takim adresem email już istnieje.");
            }

            var newPatient = new Patient
            {
                Id = _db.GetNextPatientId(),
                FirstName = createPatientDto.FirstName,
                LastName = createPatientDto.LastName,
                Email = createPatientDto.Email
            };

            _db.Patients.Add(newPatient);

            var patientDto = new PatientDto
            {
                Id = newPatient.Id,
                FirstName = newPatient.FirstName,
                LastName = newPatient.LastName,
                Email = newPatient.Email
            };

            return CreatedAtAction(nameof(GetPatient), new { id = patientDto.Id }, patientDto);
        }

        // aktualizacja danych istniejącego pacjenta
        [HttpPut("{id}")]
        public IActionResult UpdatePatient(int id, CreatePatientDto updatePatientDto)
        {
            var patient = _db.Patients.FirstOrDefault(p => p.Id == id);
            if (patient == null)
            {
                return NotFound();
            }

            // przy zmianie maila sprawdzamy czy inny pacjent go nie ma
            if (_db.Patients.Any(p => p.Id != id && p.Email.Equals(updatePatientDto.Email, StringComparison.OrdinalIgnoreCase)))
            {
                return BadRequest("Inny pacjent już używa tego adresu email.");
            }

            patient.FirstName = updatePatientDto.FirstName;
            patient.LastName = updatePatientDto.LastName;
            patient.Email = updatePatientDto.Email;

            return NoContent();
        }

        // usuwanie pacjenta razem z jego wizytami
        [HttpDelete("{id}")]
        public IActionResult DeletePatient(int id)
        {
            var patient = _db.Patients.FirstOrDefault(p => p.Id == id);
            if (patient == null)
            {
                return NotFound();
            }

            _db.Patients.Remove(patient);
            
            // czyścimy też wizyty tego pacjenta
            _db.Appointments.RemoveAll(a => a.PatientId == id);

            return NoContent();
        }
    }
}
