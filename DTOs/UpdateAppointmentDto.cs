using System.ComponentModel.DataAnnotations;

namespace VisitBookingSystem.DTOs
{
    /// <summary>Dane wejściowe do aktualizacji istniejącej wizyty.</summary>
    public class UpdateAppointmentDto
    {
        /// <summary>Nowa data i godzina wizyty. Musi być datą przyszłą.</summary>
        [Required(ErrorMessage = "Data wizyty jest wymagana.")]
        public DateTime VisitDate { get; set; }

        /// <summary>Opcjonalny opis wizyty (max 200 znaków).</summary>
        [StringLength(200, ErrorMessage = "Opis nie może przekraczać 200 znaków.")]
        public string? Description { get; set; }

        /// <summary>Czy wizyta ma zostać anulowana. Nie można anulować wizyty na mniej niż 2 godziny przed terminem.</summary>
        public bool IsCancelled { get; set; }
    }
}
