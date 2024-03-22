using HospitalPaymentSystem.Core.Abstractions;
using HospitalPaymentSystem.Core.Dtos;
using HospitalPaymentSystem.Core.Utilities;
using HospitalPaymentSystem.Domain.Entities;

namespace HospitalPaymentSystem.Core.Services
{
    public class SearchService : ISearchService
    {
        private readonly IRepository _repository;

        public SearchService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<PaginatorDTO<IEnumerable<PatientDTO>>> SearchPatientAsync(string searchTerm, PaginationFilter paginationFilter)
        {
            var patients = await _repository.SearchPatientsAsync(searchTerm);

            var paginatedPatients = PaginatePatients(patients, paginationFilter);

            return paginatedPatients;
        }

        private PaginatorDTO<IEnumerable<PatientDTO>> PaginatePatients(IEnumerable<Patient> patients, PaginationFilter paginationFilter)
        {
            int totalItems = patients.Count();
            int totalPages = (int)Math.Ceiling(totalItems / (double)paginationFilter.PageSize);
            var pagedPatients = patients
                .Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                .Take(paginationFilter.PageSize)
                .Select(p => new PatientDTO
                {
                    PatientID = p.PatientID,
                    FirstName = p.FirstName,
                    LastName = p.LastName,
                    ContactNumber = p.ContactNumber,
                    Email = p.Email,
                    Payments = p.Payments.Select(payment => new PaymentDTO
                    {
                        PaymentID = payment.PaymentID,
                        PatientID = payment.PatientID,
                        Amount = payment.Amount,
                        PaymentDate = payment.PaymentDate
                    }).ToList()
                });

            var paginatorDTO = new PaginatorDTO<IEnumerable<PatientDTO>>
            {
                PageItems = pagedPatients,
                PageSize = paginationFilter.PageSize,
                CurrentPage = paginationFilter.PageNumber,
                NumberOfPages = totalPages
            };

            return paginatorDTO;
        }
    }
}
