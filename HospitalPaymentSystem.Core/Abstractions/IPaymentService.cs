using HospitalPaymentSystem.Core.Dtos;

namespace HospitalPaymentSystem.Core.Abstractions
{
    public interface IPaymentService
    {
        Task<bool> AddPaymentAsync(PaymentDTO paymentDTO);
    }
}
