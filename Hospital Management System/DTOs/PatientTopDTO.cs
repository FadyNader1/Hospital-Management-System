using System.ComponentModel.DataAnnotations;

namespace Hospital_Management_System.DTOs
{
    public class PatientTopDTO
    {
        [Required]
        public PatientDTO patientDTO { get; set; }
        public NewPatientDTO? newPatientDTO { get; set; }  
    }
}

