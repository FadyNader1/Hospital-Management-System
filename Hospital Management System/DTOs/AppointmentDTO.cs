using Hospital_Management_System.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace Hospital_Management_System.DTOs
{
    public class AppointmentDTO
    {
        [Required]
        public int PatientId { get; set; }
        [Required]
        public int DoctorId { get; set; }
        [Required]
        public string Reason { get; set; }
        [Required]
        public DateTime AppointmentDate { get; set; }
        public AppointmentStatus Status { get; set; } = AppointmentStatus.Pending;// Default status is Pending
    }
}
