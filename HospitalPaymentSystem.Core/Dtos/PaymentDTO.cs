using System.ComponentModel.DataAnnotations;

namespace HospitalPaymentSystem.Core.Dtos
{
    public class PaymentDTO
    {
        public int PaymentID { get; set; }

        [Required(ErrorMessage = "Patient ID is required.")]
        public int PatientID { get; set; }

        [Required(ErrorMessage = "Amount is required.")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Amount must be greater than 0.")]
        public decimal Amount { get; set; }

        [Required(ErrorMessage = "Payment date is required.")]
        public DateTime PaymentDate { get; set; }
    }
}
