namespace MvcSchool.Models
{
    public class UpdateClassViewModel
    {
        public int ClassID { get; set; }
        public string ClassName { get; set; }
        public double ClassFee { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}
