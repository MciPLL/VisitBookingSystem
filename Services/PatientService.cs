using VisitBookingSystem.Data;
using VisitBookingSystem.DTOs;
using VisitBookingSystem.Exceptions;
using VisitBookingSystem.Models;

namespace VisitBookingSystem.Services
{
    public class PatientService : IPatientService
    {
        private readonly IInMemoryDatabase _db;

        public PatientService(IInMemoryDatabase db)
        {
            _db = db;
        }

        public IEnumerable<PatientDto> GetAll()
        {
            return _db.Patients.Select(p => new PatientDto
            {
                Id = p.Id,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email
            });
        }

        public PatientDto GetById(int id)
        {
            var patient = _db.Patients.FirstOrDefault(p => p.Id == id) 
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
            if (_db.Patients.Any(p => p.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase)))
            {
                throw new BusinessRuleException("Pacjent z takim adresem email już istnieje.");
            }

            var newPatient = new Patient
            {
                Id = _db.GetNextPatientId(),
                FirstName = dto.FirstName,
                LastName = dto.LastName,
                Email = dto.Email
            };

            _db.Patients.Add(newPatient);

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
            var patient = _db.Patients.FirstOrDefault(p => p.Id == id)
                ?? throw new NotFoundException($"Pacjent o Id {id} nie istnieje.");

            if (_db.Patients.Any(p => p.Id != id && p.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase)))
            {
                throw new BusinessRuleException("Inny pacjent już używa tego adresu email.");
            }

            patient.FirstName = dto.FirstName;
            patient.LastName = dto.LastName;
            patient.Email = dto.Email;
        }

        public void Delete(int id)
        {
            var patient = _db.Patients.FirstOrDefault(p => p.Id == id)
                ?? throw new NotFoundException($"Pacjent o Id {id} nie istnieje.");

            _db.Patients.Remove(patient);
            _db.Appointments.RemoveAll(a => a.PatientId == id);
        }
    }
}