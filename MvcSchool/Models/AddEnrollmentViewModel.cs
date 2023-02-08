namespace MvcSchool.Models
{
    public class AddEnrollmentViewModel
    {
        public int StudentID { get; set; }
        public int ClassID { get; set; }
        public string PaymentStautus { get; set; }
        public DateTime PaymentDeadline { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}
