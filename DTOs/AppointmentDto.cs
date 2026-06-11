namespace VisitBookingSystem.DTOs
{
    /// <summary>Dane wizyty zwracane przez API.</summary>
    public class AppointmentDto
    {
        /// <summary>Unikalny identyfikator wizyty.</summary>
        public required int Id { get; init; }

        /// <summary>Identyfikator pacjenta powiązanego z wizytą.</summary>
        public required int PatientId { get; init; }

        /// <summary>Data i godzina wizyty.</summary>
        public required DateTime VisitDate { get; init; }

        /// <summary>Opcjonalny opis wizyty (max 200 znaków).</summary>
        public string? Description { get; init; }

        /// <summary>Czy wizyta została anulowana przez pacjenta.</summary>
        public required bool IsCancelled { get; init; }
    }
}
