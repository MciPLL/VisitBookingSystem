namespace VisitBookingSystem.DTOs
{
    public class AppointmentDto
    {
        public required int Id { get; init; }
        public required int PatientId { get; init; }
        public required DateTime VisitDate { get; init; }
        public string? Description { get; init; }
        public required bool IsCancelled { get; init; }
    }
}
