using System.Text;
using Microsoft.EntityFrameworkCore;
using VisitBookingSystem.Data;
using VisitBookingSystem.DTOs;
using VisitBookingSystem.Exceptions;
using VisitBookingSystem.Models;

namespace VisitBookingSystem.Services
{
    public class PatientService : IPatientService
    {
        private readonly AppDbContext _context;

        public PatientService(AppDbContext context)
        {
            _context = context;
        }

        public IEnumerable<PatientDto> GetAll(string? sortBy = null, string? sortOrder = null)
        {
            var query = _context.Patients.AsQueryable();

            bool desc = sortOrder?.ToLower() == "desc";
            query = sortBy?.ToLower() switch
            {
                "firstname" => desc ? query.OrderByDescending(p => p.FirstName) : query.OrderBy(p => p.FirstName),
                "email"     => desc ? query.OrderByDescending(p => p.Email)     : query.OrderBy(p => p.Email),
                _           => desc ? query.OrderByDescending(p => p.LastName)  : query.OrderBy(p => p.LastName)
            };

            return query.Select(p => new PatientDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email
            });
        }

        public PatientDto GetById(int id)
        {
            var patient = _context.Patients.FirstOrDefault(p => p.Id == id)
                ?? throw new NotFoundException($"Pacjent o Id {id} nie istnieje.");

            return new PatientDto
            {
                Id = patient.Id,
                FirstName = patient.FirstName,
                LastName = patient.LastName,
                Email = patient.Email
            };
        }

        public PatientDto Create(CreatePatientDto dto)
        {
            if (_context.Patients.Any(p => p.Email.ToLower() == dto.Email.ToLower()))
                throw new BusinessRuleException("Pacjent z takim adresem email już istnieje.");

            var newPatient = new Patient
            {
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email
            };

            _context.Patients.Add(newPatient);
            _context.SaveChanges();

            return new PatientDto
            {
                Id = newPatient.Id,
                FirstName = newPatient.FirstName,
                LastName = newPatient.LastName,
                Email = newPatient.Email
            };
        }

        public void Update(int id, CreatePatientDto dto)
        {
            var patient = _context.Patients.FirstOrDefault(p => p.Id == id)
                ?? throw new NotFoundException($"Pacjent o Id {id} nie istnieje.");

            if (_context.Patients.Any(p => p.Id != id && p.Email.ToLower() == dto.Email.ToLower()))
                throw new BusinessRuleException("Inny pacjent już używa tego adresu email.");

            patient.FirstName = dto.FirstName;
            patient.LastName = dto.LastName;
            patient.Email = dto.Email;
            _context.SaveChanges();
        }

        public void Delete(int id)
        {
            var patient = _context.Patients.FirstOrDefault(p => p.Id == id)
                ?? throw new NotFoundException($"Pacjent o Id {id} nie istnieje.");

            var appointments = _context.Appointments.Where(a => a.PatientId == id).ToList();
            _context.Appointments.RemoveRange(appointments);
            _context.Patients.Remove(patient);
            _context.SaveChanges();
        }

        public string ExportToCsv()
        {
            var sb = new StringBuilder();
            sb.AppendLine("Id;FirstName;LastName;Email");

            foreach (var p in _context.Patients)
            {
                sb.AppendLine($"{p.Id};{EscapeCsv(p.FirstName)};{EscapeCsv(p.LastName)};{EscapeCsv(p.Email)}");
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
