using System.Text;
using Microsoft.EntityFrameworkCore;
using VisitBookingSystem.Data;
using VisitBookingSystem.DTOs;
using VisitBookingSystem.Exceptions;
using VisitBookingSystem.Models;

namespace VisitBookingSystem.Services
{
    public class AppointmentService : IAppointmentService
    {
        private readonly AppDbContext _context;

        public AppointmentService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<AppointmentDto> GetAll(string? sortBy = null, string? sortOrder = null)
        {
            var query = _context.Appointments.AsQueryable();

            bool desc = sortOrder?.ToLower() == "desc";
            query = sortBy?.ToLower() switch
            {
                "patientid"   => desc ? query.OrderByDescending(a => a.PatientId)   : query.OrderBy(a => a.PatientId),
                "iscancelled" => desc ? query.OrderByDescending(a => a.IsCancelled) : query.OrderBy(a => a.IsCancelled),
                _             => desc ? query.OrderByDescending(a => a.VisitDate)   : query.OrderBy(a => a.VisitDate)
            };

            return query.Select(a => new AppointmentDto
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
            var appointment = _context.Appointments.FirstOrDefault(a => a.Id == id)
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
            if (dto.VisitDate < DateTime.Now)
                throw new BusinessRuleException("Nie można zarezerwować wizyty z datą przeszłą.");

            if (!_context.Patients.Any(p => p.Id == dto.PatientId))
                throw new NotFoundException($"Pacjent o Id {dto.PatientId} nie istnieje.");

            // REGUŁA 1: Limit rezerwacji (max 3 aktywne)
            var activeCount = _context.Appointments.Count(a => a.PatientId == dto.PatientId && !a.IsCancelled);
            if (activeCount >= 4)
                throw new BusinessRuleException("Pacjent posiada już maksymalną liczbę aktywnych rezerwacji (3).");

            // Walidacja kolizji terminów (okno ±30 minut)
            var windowStart = dto.VisitDate.AddMinutes(-30);
            var windowEnd = dto.VisitDate.AddMinutes(30);
            if (_context.Appointments.Any(a => !a.IsCancelled &&
                a.VisitDate > windowStart && a.VisitDate < windowEnd))
            {
                throw new BusinessRuleException("Ten termin jest już zajęty lub zbyt blisko innej wizyty.");
            }

            var newAppointment = new Appointment
            {
                PatientId = dto.PatientId,
                VisitDate = dto.VisitDate,
                Description = dto.Description,
                IsCancelled = false
            };

            _context.Appointments.Add(newAppointment);
            _context.SaveChanges();

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
            var appointment = _context.Appointments.FirstOrDefault(a => a.Id == id)
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

                var newWindowStart = dto.VisitDate.AddMinutes(-30);
                var newWindowEnd = dto.VisitDate.AddMinutes(30);
                if (!dto.IsCancelled && _context.Appointments.Any(a => a.Id != id && !a.IsCancelled &&
                    a.VisitDate > newWindowStart && a.VisitDate < newWindowEnd))
                {
                    throw new BusinessRuleException("Nowy termin jest już zajęty.");
                }
            }

            appointment.VisitDate = dto.VisitDate;
            appointment.Description = dto.Description;
            appointment.IsCancelled = dto.IsCancelled;
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var appointment = _context.Appointments.FirstOrDefault(a => a.Id == id)
                ?? throw new NotFoundException($"Wizyta o Id {id} nie istnieje.");

            _context.Appointments.Remove(appointment);
            _context.SaveChanges();
        }

        public string ExportToCsv()
        {
            var appointments = _context.Appointments
                .Include(a => a.Patient)
                .ToList();

            var sb = new StringBuilder();
            sb.AppendLine("Id;PatientId;PatientName;VisitDate;Description;IsCancelled");

            foreach (var a in appointments)
            {
                var patientName = a.Patient != null
                    ? $"{a.Patient.FirstName} {a.Patient.LastName}"
                    : string.Empty;

                sb.AppendLine(string.Join(";",
                    a.Id,
                    a.PatientId,
                    EscapeCsv(patientName),
                    a.VisitDate.ToString("yyyy-MM-ddTHH:mm:ss"),
                    EscapeCsv(a.Description ?? string.Empty),
                    a.IsCancelled));
            }

            return sb.ToString();
        }

        private static string EscapeCsv(string value)
        {
            if (value.Contains(',') || value.Contains('"') || value.Contains('\n'))
                return $"\"{value.Replace("\"", "\"\"")}\"";
            return value;
        }
    }
}
