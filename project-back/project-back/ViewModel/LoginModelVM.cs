using System.ComponentModel.DataAnnotations;

namespace project_back.ViewModel
{
    public class LoginModelVM
    {
        [Required()]
        public string Login { get; set; }
        [Required()]
        public string Password { get; set; }
    }
}
