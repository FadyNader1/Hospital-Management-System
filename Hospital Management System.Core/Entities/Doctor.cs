using Hospital_Management_System.Core.Entities.Identity;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital_Management_System.Core.Entities
{
    public class Doctor: BaseEntity
    {
        public string FName { get; set; }
        public string LName { get; set; }
        public string? country { get; set; }
        public string? city { get; set; }
        public string? Gender { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string? ProfileImageUrl { get; set; }
        public string Specialization { get; set; }
        [ForeignKey("UserApp")]
        public string UserId { get; set; } // Foreign key to UserApp entity
        public UserApp UserApp { get; set; } // Navigation property to UserApp entity
        // Navigation properties
        public ICollection<Appointment> Appointments { get; set; } = new HashSet<Appointment>();
        public ICollection<MedicalReport> MedicalReports { get; set; } = new HashSet<MedicalReport>();
        public ICollection<DoctorAvailability> DoctorAvailabilities { get; set; } = new HashSet<DoctorAvailability>();

    }
}
