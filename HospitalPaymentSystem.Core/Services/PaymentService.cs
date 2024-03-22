using HospitalPaymentSystem.Core.Abstractions;
using HospitalPaymentSystem.Core.Dtos;
using HospitalPaymentSystem.Domain.Entities;

namespace HospitalPaymentSystem.Core.Services
{
    public class PaymentService : IPaymentService
    {
        private readonly IRepository _repository;

        public PaymentService(IRepository repository)
        {
            _repository = repository;
        }

        public async Task<bool> AddPaymentAsync(PaymentDTO paymentDTO)
        {
            try
            {
                var payment = new Payment
                {
                    PatientID = paymentDTO.PatientID,
                    Amount = paymentDTO.Amount,
                    PaymentDate = paymentDTO.PaymentDate
                };
                return await _repository.PostPaymentAsync(payment);
            }
            catch (Exception ex)
            {
                
                return false;
            }
        }
    }
}
