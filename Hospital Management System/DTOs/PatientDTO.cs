using System.ComponentModel.DataAnnotations;

namespace Hospital_Management_System.DTOs
{
    public class PatientDTO
    {
        [Required]
        public string Name { get; set; }
        [Required]
        public int Age { get; set; }
        [Required]
        public string PhoneNumber { get; set; }
        [Required]
        public string ChronicDiseases { get; set; }
    }
}
