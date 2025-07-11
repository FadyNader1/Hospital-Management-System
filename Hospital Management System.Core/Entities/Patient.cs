using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital_Management_System.Core.Entities
{
    public class Patient: BaseEntity
    {
        public string Name { get; set; }
        public int Age { get; set; }
        public string PhoneNumber { get; set; }
        public string ChronicDiseases { get; set; }
        // Navigation properties
        public ICollection<Appointment> Appointments { get; set; }= new HashSet<Appointment>();
        public ICollection<MedicalReport> MedicalReports { get; set; } = new HashSet<MedicalReport>();

    }
}
