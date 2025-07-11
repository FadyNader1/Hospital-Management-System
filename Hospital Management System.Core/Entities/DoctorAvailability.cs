using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital_Management_System.Core.Entities
{
    public class DoctorAvailability:BaseEntity
    {
        public int doctorid { get; set; }
        public Doctor doctor { get; set; }
        public DayOfWeek Day { get; set; }
        public TimeSpan AvailableFrom { get; set; }
        public TimeSpan AvailableTo { get; set; }
    }
}
