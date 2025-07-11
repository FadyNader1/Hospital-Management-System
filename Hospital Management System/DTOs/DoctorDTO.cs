using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Hospital_Management_System.DTOs
{
    public class DoctorDTO
    {
        public string FName { get; set; }
        public string LName { get; set; }
        public string Email { get; set; }   
        public string? country { get; set; }
        public string? city { get; set; }
        public string? Gender { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string Specialization { get; set; }
        //[ForeignKey("UserApp")]
        //public string UserId { get; set; }
        [Required]
        public List<DoctorAvailabilityDTO> doctorAvailability { get; set; }
        [Required]
        [DataType(DataType.Password)]
        public string Password { get; set; }
        [Required]
        [DataType(DataType.Password)]
        [Compare("Password", ErrorMessage = "Confirm Password does not match with Password")]
        public string ConfirmPassword { get; set; }

    }
}
