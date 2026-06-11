using System.ComponentModel.DataAnnotations;

namespace VisitBookingSystem.DTOs
{
    /// <summary>Dane pacjenta zwracane przez API.</summary>
    public class PatientDto
    {
        /// <summary>Unikalny identyfikator pacjenta.</summary>
        [Required]
        public required int Id { get; init; }

        /// <summary>Imię pacjenta.</summary>
        [Required(ErrorMessage = "Imię jest wymagane")]
        public required string FirstName { get; init; }

        /// <summary>Nazwisko pacjenta.</summary>
        [Required(ErrorMessage = "Nazwisko jest wymagane")]
        public required string LastName { get; init; }

        /// <summary>Adres e-mail pacjenta.</summary>
        [Required(ErrorMessage = "Email jest wymagany")]
        public required string Email { get; init; }
    }
}
