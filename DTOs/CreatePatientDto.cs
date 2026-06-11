using System.ComponentModel.DataAnnotations;

namespace VisitBookingSystem.DTOs
{
    /// <summary>Dane wejściowe do utworzenia lub aktualizacji pacjenta.</summary>
    public class CreatePatientDto
    {
        /// <summary>Imię pacjenta (2–50 znaków).</summary>
        [Required(ErrorMessage = "Imię jest wymagane.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Imię musi mieć od 2 do 50 znaków.")]
        public required string FirstName { get; set; }

        /// <summary>Nazwisko pacjenta (2–50 znaków).</summary>
        [Required(ErrorMessage = "Nazwisko jest wymagane.")]
        [StringLength(50, MinimumLength = 2, ErrorMessage = "Nazwisko musi mieć od 2 do 50 znaków.")]
        public required string LastName { get; set; }

        /// <summary>Adres e-mail pacjenta. Musi być unikalny w systemie.</summary>
        [Required(ErrorMessage = "Email jest wymagany.")]
        [EmailAddress(ErrorMessage = "Niepoprawny format adresu email.")]
        public required string Email { get; set; }
    }
}
