using System.ComponentModel.DataAnnotations;

namespace Menu.Models
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Username is required")]
       // [Display(Name = "User Name")]
        public string Username { get; set; }

        [Required]
        [DataType(DataType.Password)]
        //[Display (Name = "Password")]
        public string Password { get; set; }

    }
}
