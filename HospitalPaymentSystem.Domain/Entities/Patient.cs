

namespace HospitalPaymentSystem.Domain.Entities
{
    public class Patient
    {
        public int PatientID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string ContactNumber { get; set; }
        public string Email { get; set; }
        public List<Payment> Payments { get; set; }
    }
}
