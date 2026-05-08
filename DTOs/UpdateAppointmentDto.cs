using System.ComponentModel.DataAnnotations;

namespace VisitBookingSystem.DTOs
{
    public class UpdateAppointmentDto
    {
        [Required(ErrorMessage = "Data wizyty jest wymagana")]
        public required DateTime VisitDate { get; init; }

        [MaxLength(500)]
        public string? Description { get; init; }

        public bool IsCancelled { get; init; }
    }
}
