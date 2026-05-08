using Microsoft.AspNetCore.Mvc;
using VisitBookingSystem.Data;
using VisitBookingSystem.DTOs;
using VisitBookingSystem.Models;

namespace VisitBookingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IInMemoryDatabase _db;

        public AppointmentsController(IInMemoryDatabase db)
        {
            _db = db;
        }

        // pobieramy wszystkie zaplanowane wizyty
        [HttpGet]
        public ActionResult<IEnumerable<AppointmentDto>> GetAppointments()
        {
            return Ok(_db.Appointments.Select(a => new AppointmentDto
            {
                Id = a.Id,
                PatientId = a.PatientId,
                VisitDate = a.VisitDate,
                Description = a.Description,
                IsCancelled = a.IsCancelled
            }));
        }

        // sprawdzamy szczegóły jednej konkretnej wizyty
        [HttpGet("{id}")]
        public ActionResult<AppointmentDto> GetAppointment(int id)
        {
            var appointment = _db.Appointments.FirstOrDefault(a => a.Id == id);

            if (appointment == null)
            {
                return NotFound();
            }

            return Ok(new AppointmentDto
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                VisitDate = appointment.VisitDate,
                Description = appointment.Description,
                IsCancelled = appointment.IsCancelled
            });
        }

        // rezerwacja nowego terminu wizyty
        [HttpPost]
        public ActionResult<AppointmentDto> CreateAppointment(CreateAppointmentDto createAppointmentDto)
        {
            // nie można rezerwować wizyt wstecz
            if (createAppointmentDto.VisitDate < DateTime.Now)
            {
                return BadRequest("Nie można zarezerwować wizyty z datą przeszłą.");
            }

            // czy pacjent na pewno jest w bazie?
            if (!_db.Patients.Any(p => p.Id == createAppointmentDto.PatientId))
            {
                return BadRequest("Pacjent o podanym Id nie istnieje.");
            }

            // sprawdzamy czy termin nie jest zajęty (wizyta trwa 30 min)
            if (_db.Appointments.Any(a => !a.IsCancelled && 
                Math.Abs((a.VisitDate - createAppointmentDto.VisitDate).TotalMinutes) < 30))
            {
                return BadRequest("Ten termin jest już zajęty lub zbyt blisko innej wizyty.");
            }

            var newAppointment = new Appointment
            {
                Id = _db.GetNextAppointmentId(),
                PatientId = createAppointmentDto.PatientId,
                VisitDate = createAppointmentDto.VisitDate,
                Description = createAppointmentDto.Description,
                IsCancelled = false
            };

            _db.Appointments.Add(newAppointment);

            var appointmentDto = new AppointmentDto
            {
                Id = newAppointment.Id,
                PatientId = newAppointment.PatientId,
                VisitDate = newAppointment.VisitDate,
                Description = newAppointment.Description,
                IsCancelled = newAppointment.IsCancelled
            };

            return CreatedAtAction(nameof(GetAppointment), new { id = appointmentDto.Id }, appointmentDto);
        }

        // zmiana daty lub statusu istniejącej wizyty
        [HttpPut("{id}")]
        public IActionResult UpdateAppointment(int id, UpdateAppointmentDto updateAppointmentDto)
        {
            var appointment = _db.Appointments.FirstOrDefault(a => a.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            // walidacja daty (chyba że właśnie anulujemy wizytę)
            if (updateAppointmentDto.VisitDate < DateTime.Now && !updateAppointmentDto.IsCancelled)
            {
                return BadRequest("Nie można zmienić daty wizyty na przeszłą.");
            }

            // sprawdzamy czy nowy termin z nikim nie koliduje
            if (!updateAppointmentDto.IsCancelled && 
                _db.Appointments.Any(a => a.Id != id && !a.IsCancelled && 
                Math.Abs((a.VisitDate - updateAppointmentDto.VisitDate).TotalMinutes) < 30))
            {
                return BadRequest("Nowy termin jest już zajęty lub zbyt blisko innej wizyty.");
            }

            appointment.VisitDate = updateAppointmentDto.VisitDate;
            appointment.Description = updateAppointmentDto.Description;
            appointment.IsCancelled = updateAppointmentDto.IsCancelled;

            return NoContent();
        }

        // całkowite usunięcie wizyty z systemu
        [HttpDelete("{id}")]
        public IActionResult DeleteAppointment(int id)
        {
            var appointment = _db.Appointments.FirstOrDefault(a => a.Id == id);
            if (appointment == null)
            {
                return NotFound();
            }

            _db.Appointments.Remove(appointment);

            return NoContent();
        }
    }
}