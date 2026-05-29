using System.ComponentModel.DataAnnotations;

namespace VisitBookingSystem.DTOs
{
    public class CreateAppointmentDto
    {
        [Required(ErrorMessage = "Id pacjenta jest wymagane.")]
        public int PatientId { get; set; }

        [Required(ErrorMessage = "Data wizyty jest wymagana.")]
        public DateTime VisitDate { get; set; }

        [StringLength(200, ErrorMessage = "Opis nie może przekraczać 200 znaków.")]
        public string? Description { get; set; }
    }
}
