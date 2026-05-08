using VisitBookingSystem.Models;

namespace VisitBookingSystem.Data
{
    public interface IInMemoryDatabase
    {
        List<Patient> Patients { get; }
        List<Appointment> Appointments { get; }
        int GetNextPatientId();
        int GetNextAppointmentId();
    }

    public class InMemoryDatabase : IInMemoryDatabase
    {
        private readonly List<Patient> _patients = new List<Patient>
        {
            new Patient { Id = 1, FirstName = "Jan", LastName = "Kowalski", Email = "jan.kowalski@example.com" },
            new Patient { Id = 2, FirstName = "Anna", LastName = "Nowak", Email = "anna.nowak@example.com" }
        };

        private readonly List<Appointment> _appointments = new List<Appointment>
        {
            new Appointment { Id = 1, PatientId = 1, VisitDate = DateTime.Now.AddDays(1), Description = "Kontrola", IsCancelled = false },
            new Appointment { Id = 2, PatientId = 2, VisitDate = DateTime.Now.AddDays(2), Description = "Konsultacja", IsCancelled = false }
        };

        private int _lastPatientId = 2;
        private int _lastAppointmentId = 2;
        private readonly object _lock = new object();

        public List<Patient> Patients => _patients;
        public List<Appointment> Appointments => _appointments;

        public int GetNextPatientId()
        {
            lock (_lock)
            {
                return ++_lastPatientId;
            }
        }

        public int GetNextAppointmentId()
        {
            lock (_lock)
            {
                return ++_lastAppointmentId;
            }
        }
    }
}
