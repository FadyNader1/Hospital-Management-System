using System.ComponentModel.DataAnnotations;

namespace Hospital_Management_System.DTOs.IdentityDTO
{
    public class RegisterDTO
    {
        [Required(ErrorMessage = "First name is required.")]
        [MaxLength(50, ErrorMessage = "First name cannot exceed 50 characters.")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Last name is required.")]
        [MaxLength(50, ErrorMessage = "Last name cannot exceed 50 characters.")]
        public string LastName { get; set; }
        [MaxLength(50, ErrorMessage = "Country cannot exceed 50 characters.")]
        public string? Country { get; set; }
        [MaxLength(50, ErrorMessage = "City cannot exceed 50 characters.")]
        public string? City { get; set; }
        public string? Gender { get; set; }
        [DataType(DataType.Date,ErrorMessage = " DateOfBirth data type must be Date.")]
        public DateOnly DateOfBirth { get; set; }
        
        public IFormFile? ProfileImage { get; set; }

        [Required(ErrorMessage = "Email is required.")]
        [DataType(DataType.EmailAddress)]
        public string Email { get; set; }
        [Required(ErrorMessage = "Password is required.")]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required(ErrorMessage = "Confirm password is required.")]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Passwords do not match.")]
        public string ConfirmPassword { get; set; }
        public string? PhoneNumber { get; set; }
    }
}