using Hospital_Management_System.Core.Entities;
using System.ComponentModel.DataAnnotations;

namespace Hospital_Management_System.DTOs
{
    public class AppointmentDetailsDTO
    {
        public int id { get; set; }
        public string Reason { get; set; }
        public DateTime AppointmentDate { get; set; }
        public PatientBasicDTO Patient { get; set; }
        public DoctorBasicDTO Doctor { get; set; }
        public AppointmentStatus Status { get; set; } // Default status is Pending

    }
}
