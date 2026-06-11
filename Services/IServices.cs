using VisitBookingSystem.DTOs;

namespace VisitBookingSystem.Services
{
    public interface IPatientService
    {
        IEnumerable<PatientDto> GetAll(string? sortBy = null, string? sortOrder = null);
        PatientDto GetById(int id);
        PatientDto Create(CreatePatientDto dto);
        void Update(int id, CreatePatientDto dto);
        void Delete(int id);
        string ExportToCsv();
    }

    public interface IAppointmentService
    {
        IEnumerable<AppointmentDto> GetAll(string? sortBy = null, string? sortOrder = null);
        AppointmentDto GetById(int id);
        AppointmentDto Create(CreateAppointmentDto dto);
        void Update(int id, UpdateAppointmentDto dto);
        void Delete(int id);
        string ExportToCsv();
    }
}