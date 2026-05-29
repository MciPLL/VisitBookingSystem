using VisitBookingSystem.Data;
using VisitBookingSystem.DTOs;
using VisitBookingSystem.Exceptions;
using VisitBookingSystem.Models;

namespace VisitBookingSystem.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly IInMemoryDatabase _db;

        public AppointmentService(IInMemoryDatabase db)
        {
            _db = db;
        }

        public IEnumerable<AppointmentDto> GetAll()
        {
            return _db.Appointments.Select(a => new AppointmentDto
            {
                Id = a.Id,
                PatientId = a.PatientId,
                VisitDate = a.VisitDate,
                Description = a.Description,
                IsCancelled = a.IsCancelled
            });
        }

        public AppointmentDto GetById(int id)
        {
            var appointment = _db.Appointments.FirstOrDefault(a => a.Id == id)
                ?? throw new NotFoundException($"Wizyta o Id {id} nie istnieje.");

            return new AppointmentDto
            {
                Id = appointment.Id,
                PatientId = appointment.PatientId,
                VisitDate = appointment.VisitDate,
                Description = appointment.Description,
                IsCancelled = appointment.IsCancelled
            };
        }

        public AppointmentDto Create(CreateAppointmentDto dto)
        {
            // Walidacja podstawowa
            if (dto.VisitDate < DateTime.Now)
                throw new BusinessRuleException("Nie można zarezerwować wizyty z datą przeszłą.");

            if (!_db.Patients.Any(p => p.Id == dto.PatientId))
                throw new NotFoundException($"Pacjent o Id {dto.PatientId} nie istnieje.");

            // REGUŁA 1: Limit rezerwacji (max 3 aktywne)
            var activeAppointmentsCount = _db.Appointments.Count(a => a.PatientId == dto.PatientId && !a.IsCancelled);
            if (activeAppointmentsCount >= 4)
                throw new BusinessRuleException("Pacjent posiada już maksymalną liczbę aktywnych rezerwacji (3).");

            // Walidacja kolizji terminów
            if (_db.Appointments.Any(a => !a.IsCancelled && 
                Math.Abs((a.VisitDate - dto.VisitDate).TotalMinutes) < 30))
            {
                throw new BusinessRuleException("Ten termin jest już zajęty lub zbyt blisko innej wizyty.");
            }

            var newAppointment = new Appointment
            {
                Id = _db.GetNextAppointmentId(),
                PatientId = dto.PatientId,
                VisitDate = dto.VisitDate,
                Description = dto.Description,
                IsCancelled = false
            };

            _db.Appointments.Add(newAppointment);

            return new AppointmentDto
            {
                Id = newAppointment.Id,
                PatientId = newAppointment.PatientId,
                VisitDate = newAppointment.VisitDate,
                Description = newAppointment.Description,
                IsCancelled = newAppointment.IsCancelled
            };
        }

        public void Update(int id, UpdateAppointmentDto dto)
        {
            var appointment = _db.Appointments.FirstOrDefault(a => a.Id == id)
                ?? throw new NotFoundException($"Wizyta o Id {id} nie istnieje.");

            // REGUŁA 2: Blokada anulowania "Last Minute" (2h)
            if (dto.IsCancelled && !appointment.IsCancelled)
            {
                if (appointment.VisitDate < DateTime.Now.AddHours(2))
                    throw new BusinessRuleException("Nie można anulować wizyty na mniej niż 2 godziny przed terminem.");
            }

            // Walidacja zmiany daty
            if (appointment.VisitDate != dto.VisitDate)
            {
                if (dto.VisitDate < DateTime.Now)
                    throw new BusinessRuleException("Nie można zmienić daty wizyty na przeszłą.");

                if (!dto.IsCancelled && _db.Appointments.Any(a => a.Id != id && !a.IsCancelled && 
                    Math.Abs((a.VisitDate - dto.VisitDate).TotalMinutes) < 30))
                {
                    throw new BusinessRuleException("Nowy termin jest już zajęty.");
                }
            }

            appointment.VisitDate = dto.VisitDate;
            appointment.Description = dto.Description;
            appointment.IsCancelled = dto.IsCancelled;
        }

        public void Delete(int id)
        {
            var appointment = _db.Appointments.FirstOrDefault(a => a.Id == id)
                ?? throw new NotFoundException($"Wizyta o Id {id} nie istnieje.");

            _db.Appointments.Remove(appointment);
        }
    }
}