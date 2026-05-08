using System.ComponentModel.DataAnnotations;

namespace VisitBookingSystem.DTOs
{
    public class CreateAppointmentDto
    {
        [Required(ErrorMessage = "Id pacjenta jest wymagane")]
        public required int PatientId { get; init; }

        [Required(ErrorMessage = "Data wizyty jest wymagana")]
        public required DateTime VisitDate { get; init; }

        [MaxLength(500)]
        public string? Description { get; init; }
    }
}
