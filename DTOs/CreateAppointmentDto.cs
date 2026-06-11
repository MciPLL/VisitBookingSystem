using System.ComponentModel.DataAnnotations;

namespace VisitBookingSystem.DTOs
{
    /// <summary>Dane wejściowe do utworzenia nowej wizyty.</summary>
    public class CreateAppointmentDto
    {
        /// <summary>Identyfikator pacjenta, dla którego rezerwowana jest wizyta.</summary>
        [Required(ErrorMessage = "Id pacjenta jest wymagane.")]
        public int PatientId { get; set; }

        /// <summary>Data i godzina wizyty. Musi być datą przyszłą.</summary>
        [Required(ErrorMessage = "Data wizyty jest wymagana.")]
        public DateTime VisitDate { get; set; }

        /// <summary>Opcjonalny opis wizyty (max 200 znaków).</summary>
        [StringLength(200, ErrorMessage = "Opis nie może przekraczać 200 znaków.")]
        public string? Description { get; set; }
    }
}
