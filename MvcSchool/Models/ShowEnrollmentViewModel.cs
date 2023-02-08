namespace MvcSchool.Models
{
    public class ShowEnrollmentViewModel
    { 

        public int EnrollmentID { get; set; }
        public string StudentName { get; set; }
        public string ClassName { get; set; }
        public string PaymentStautus { get; set; }
        public DateTime PaymentDeadline { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }
}
