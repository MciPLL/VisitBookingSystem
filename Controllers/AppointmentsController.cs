using System.Text;
using Microsoft.AspNetCore.Mvc;
using VisitBookingSystem.DTOs;
using VisitBookingSystem.Services;

namespace VisitBookingSystem.Controllers
{
    /// <summary>
    /// Zarządzanie wizytami.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        /// <summary>Pobiera listę wszystkich wizyt z opcjonalnym sortowaniem.</summary>
        /// <param name="sortBy">Pole sortowania: <c>visitDate</c> (domyślnie), <c>patientId</c>, <c>isCancelled</c>.</param>
        /// <param name="sortOrder">Kierunek sortowania: <c>asc</c> (domyślnie) lub <c>desc</c>.</param>
        /// <returns>Lista wizyt.</returns>
        [HttpGet]
        [ProducesResponseType(typeof(IEnumerable<AppointmentDto>), StatusCodes.Status200OK)]
        public ActionResult<IEnumerable<AppointmentDto>> GetAppointments([FromQuery] string? sortBy = null, [FromQuery] string? sortOrder = null)
        {
            return Ok(_appointmentService.GetAll(sortBy, sortOrder));
        }

        /// <summary>Pobiera wizytę o podanym identyfikatorze.</summary>
        /// <param name="id">Identyfikator wizyty.</param>
        /// <returns>Dane wizyty.</returns>
        [HttpGet("{id}")]
        [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<AppointmentDto> GetAppointment(int id)
        {
            return Ok(_appointmentService.GetById(id));
        }

        /// <summary>Tworzy nową wizytę.</summary>
        /// <param name="createAppointmentDto">Dane nowej wizyty.</param>
        /// <returns>Utworzona wizyta.</returns>
        [HttpPost]
        [ProducesResponseType(typeof(AppointmentDto), StatusCodes.Status201Created)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public ActionResult<AppointmentDto> CreateAppointment(CreateAppointmentDto createAppointmentDto)
        {
            var appointment = _appointmentService.Create(createAppointmentDto);
            return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
        }

        /// <summary>Aktualizuje istniejącą wizytę. Umożliwia zmianę daty, opisu oraz anulowanie wizyty.</summary>
        /// <param name="id">Identyfikator wizyty.</param>
        /// <param name="updateAppointmentDto">Nowe dane wizyty.</param>
        [HttpPut("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult UpdateAppointment(int id, UpdateAppointmentDto updateAppointmentDto)
        {
            _appointmentService.Update(id, updateAppointmentDto);
            return NoContent();
        }

        /// <summary>Trwale usuwa wizytę z bazy danych.</summary>
        /// <param name="id">Identyfikator wizyty.</param>
        [HttpDelete("{id}")]
        [ProducesResponseType(StatusCodes.Status204NoContent)]
        [ProducesResponseType(StatusCodes.Status404NotFound)]
        public IActionResult DeleteAppointment(int id)
        {
            _appointmentService.Delete(id);
            return NoContent();
        }

        /// <summary>Eksportuje listę wizyt do pliku CSV.</summary>
        /// <returns>Plik CSV z danymi wizyt.</returns>
        [HttpGet("export")]
        [ProducesResponseType(typeof(FileContentResult), StatusCodes.Status200OK)]
        public IActionResult ExportAppointments()
        {
            var csv = _appointmentService.ExportToCsv();
            var bytes = new System.Text.UTF8Encoding(true).GetBytes(csv);
            return File(bytes, "text/csv; charset=utf-8", "appointments.csv");
        }
    }
}
