using System.ComponentModel.DataAnnotations;

namespace VisitBookingSystem.DTOs
{
    public class UpdateAppointmentDto
    {
        [Required(ErrorMessage = "Data wizyty jest wymagana.")]
        public DateTime VisitDate { get; set; }

        [StringLength(200, ErrorMessage = "Opis nie może przekraczać 200 znaków.")]
        public string? Description { get; set; }

        public bool IsCancelled { get; set; }
    }
}