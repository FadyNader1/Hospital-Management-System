using System.ComponentModel.DataAnnotations;

namespace Hospital_Management_System.DTOs.IdentityDTO
{
    public class LoginDTO
    {
        [Required(ErrorMessage = "Email is required.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
    }
}
