using HospitalPaymentSystem.Core.Abstractions;
using HospitalPaymentSystem.Core.Dtos;
using HospitalPaymentSystem.Core.Utilities;
using HospitalPaymentSystem.Domain.Entities;

namespace HospitalPaymentSystem.Core.Services
{
    public class PatientServices : IPatientServices
    {
        private readonly IRepository _repository;

        public PatientServices(IRepository repository)
        {
            _repository = repository;
        }

        public async Task RegisterPatientAsync(PatientDTO patientDTO)
        {
            try
            {
                var patient = new Patient
                {
                    FirstName = patientDTO.FirstName,
                    LastName = patientDTO.LastName,
                    ContactNumber = patientDTO.ContactNumber,
                    Email = patientDTO.Email
                };

                await _repository.CreatePatientAsync(patient);
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<PaginatorDTO<IEnumerable<PatientDTO>>> GetAllPatientsAsync(PaginationFilter paginationFilter)
        {
            try
            {
                var patients = await _repository.GetAllPatientsAsync();

                var patientDTOs = patients.Select(patient => new PatientDTO
                {
                    PatientID = patient.PatientID,
                    FirstName = patient.FirstName,
                    LastName = patient.LastName,
                    ContactNumber = patient.ContactNumber,
                    Email = patient.Email,
                    Payments = patient.Payments.Select(payment => new PaymentDTO
                    {
                        PaymentID = payment.PaymentID,
                        PatientID = payment.PatientID,
                        Amount = payment.Amount,
                        PaymentDate = payment.PaymentDate
                    }).ToList()
                });

                var paginatedPatients = patientDTOs
                    .Skip((paginationFilter.PageNumber - 1) * paginationFilter.PageSize)
                    .Take(paginationFilter.PageSize)
                    .ToList();

                int totalPatientsCount = patientDTOs.Count();
                int totalPages = (int)Math.Ceiling((double)totalPatientsCount / paginationFilter.PageSize);

                return new PaginatorDTO<IEnumerable<PatientDTO>>
                {
                    PageItems = paginatedPatients,
                    PageSize = paginationFilter.PageSize,
                    CurrentPage = paginationFilter.PageNumber,
                    NumberOfPages = totalPages
                };
            }
            catch (Exception ex)
            {
                throw;
            }
        }

        public async Task<PatientDTO> GetPatientByIdAsync(int patientId)
        {
            try
            {
                var patient = await _repository.GetPatientByIdAsync(patientId);

                if (patient != null)
                {
                    var patientDTO = new PatientDTO
                    {
                        PatientID = patient.PatientID,
                        FirstName = patient.FirstName,
                        LastName = patient.LastName,
                        ContactNumber = patient.ContactNumber,
                        Email = patient.Email,
                        Payments = patient.Payments.Select(payment => new PaymentDTO
                        {
                            PaymentID = payment.PaymentID,
                            PatientID = payment.PatientID,
                            Amount = payment.Amount,
                            PaymentDate = payment.PaymentDate
                        }).ToList()
                    };

                    return patientDTO;
                }
                else
                {
                    return null;
                }
            }
            catch (Exception ex)
            {
                throw;
            }
        }
    }
}
