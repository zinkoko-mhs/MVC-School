using Microsoft.AspNetCore.Mvc.Rendering;

namespace MvcSchool.Models
{
    public class AddEnrollmentViewModel
    {
        public List<string> StudentName { get; set; }
        public List<string> ClassName { get; set; }
        public string PaymentStautus {get;set;}
        public DateTime PaymentDeadline { get; set; }
        public DateTime CreatedDate { get; set; }
        public string CreatedBy { get; set; }
    }

    /*public enum PaymentStatus
    {
        Done,Pending
    }*/
}
