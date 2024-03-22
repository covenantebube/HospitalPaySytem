using HospitalPaymentSystem.Core.Dtos;
using HospitalPaymentSystem.Core.Utilities;

namespace HospitalPaymentSystem.Core.Abstractions
{
    public interface IPatientServices
    {

        Task RegisterPatientAsync(PatientDTO patientDTO);
        Task<PaginatorDTO<IEnumerable<PatientDTO>>> GetAllPatientsAsync(PaginationFilter paginationFilter);
        Task<PatientDTO> GetPatientByIdAsync(int patientId);
    }
}
