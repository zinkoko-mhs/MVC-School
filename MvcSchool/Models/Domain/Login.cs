using System.ComponentModel.DataAnnotations;

namespace MvcSchool.Models.Domain
{
    public class Login
    {
        [Key]
        public int AccountId { get; set; }
        public string Email { get; set; }
        public string Password { get; set; }
        public bool KeepLoggedIn { get; set; }
    }
}
