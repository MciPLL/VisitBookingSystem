using Microsoft.AspNetCore.Mvc;
using VisitBookingSystem.DTOs;
using VisitBookingSystem.Services;

namespace VisitBookingSystem.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AppointmentsController : ControllerBase
    {
        private readonly IAppointmentService _appointmentService;

        public AppointmentsController(IAppointmentService appointmentService)
        {
            _appointmentService = appointmentService;
        }

        [HttpGet]
        public ActionResult<IEnumerable<AppointmentDto>> GetAppointments()
        {
            return Ok(_appointmentService.GetAll());
        }

        [HttpGet("{id}")]
        public ActionResult<AppointmentDto> GetAppointment(int id)
        {
            return Ok(_appointmentService.GetById(id));
        }

        [HttpPost]
        public ActionResult<AppointmentDto> CreateAppointment(CreateAppointmentDto createAppointmentDto)
        {
            var appointment = _appointmentService.Create(createAppointmentDto);
            return CreatedAtAction(nameof(GetAppointment), new { id = appointment.Id }, appointment);
        }

        [HttpPut("{id}")]
        public IActionResult UpdateAppointment(int id, UpdateAppointmentDto updateAppointmentDto)
        {
            _appointmentService.Update(id, updateAppointmentDto);
            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult DeleteAppointment(int id)
        {
            _appointmentService.Delete(id);
            return NoContent();
        }
    }
}