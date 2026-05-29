using VisitBookingSystem.DTOs;

namespace VisitBookingSystem.Services
{
    public interface IPatientService
    {
        IEnumerable<PatientDto> GetAll();
        PatientDto GetById(int id);
        PatientDto Create(CreatePatientDto dto);
        void Update(int id, CreatePatientDto dto);
        void Delete(int id);
    }

    public interface IAppointmentService
    {
        IEnumerable<AppointmentDto> GetAll();
        AppointmentDto GetById(int id);
        AppointmentDto Create(CreateAppointmentDto dto);
        void Update(int id, UpdateAppointmentDto dto);
        void Delete(int id);
    }
}