using System.ComponentModel.DataAnnotations;

namespace VisitBookingSystem.DTOs
{
    public class CreatePatientDto
    {
        [Required(ErrorMessage = "Imię jest wymagane")]
        public required string FirstName { get; init; }

        [Required(ErrorMessage = "Nazwisko jest wymagane")]
        public required string LastName { get; init; }

        [Required(ErrorMessage = "Email jest wymagany")]
        [EmailAddress]
        public required string Email { get; init; }
    }
}
