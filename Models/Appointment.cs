namespace VisitBookingSystem.Models
{
    public class Appointment
    {
        public int Id { get; set; }
        public required int PatientId { get; set; }
        public required DateTime VisitDate { get; set; }
        public string? Description { get; set; }
        public bool IsCancelled { get; set; }
        public Patient? Patient { get; set; }
    }
}