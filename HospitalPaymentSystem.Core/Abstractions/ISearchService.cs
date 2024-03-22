using HospitalPaymentSystem.Core.Dtos;
using HospitalPaymentSystem.Core.Utilities;

namespace HospitalPaymentSystem.Core.Abstractions
{
    public interface ISearchService
    {
        Task<PaginatorDTO<IEnumerable<PatientDTO>>> SearchPatientAsync(string searchTerm, PaginationFilter paginationFilter);
    }
}
