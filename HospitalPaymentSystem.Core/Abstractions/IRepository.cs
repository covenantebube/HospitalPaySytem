using HospitalPaymentSystem.Core.Dtos;
using HospitalPaymentSystem.Domain.Entities;

namespace HospitalPaymentSystem.Core.Abstractions
{
    public interface IRepository
    {
        Task CreatePatientAsync(Patient patient);
        Task<bool> PostPaymentAsync(Payment payment);
        Task<List<Patient>> SearchPatientsAsync(string searchTerm);
        Task<List<Patient>> GetAllPatientsAsync();

        Task<Patient> GetPatientByIdAsync(int patientId);


    }
}
