using Hospital_Management_System.Core.Entities;

namespace Hospital_Management_System.DTOs
{
    public class DoctorAvailabilityDTO
    {
        public DayOfWeek Day { get; set; }
        public TimeSpan AvailableFrom { get; set; }
        public TimeSpan AvailableTo { get; set; }


    }
}
