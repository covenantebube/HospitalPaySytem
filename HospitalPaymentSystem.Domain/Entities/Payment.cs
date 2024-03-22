
namespace HospitalPaymentSystem.Domain.Entities
{
    public class Payment
    {
       public int PaymentID { get; set; }
        public int PatientID { get; set; }
        public decimal Amount { get; set; }
        public DateTime PaymentDate { get; set; }
    }
}
