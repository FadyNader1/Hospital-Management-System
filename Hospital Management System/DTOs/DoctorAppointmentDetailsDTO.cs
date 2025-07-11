namespace Hospital_Management_System.DTOs
{
    public class DoctorAppointmentDetailsDTO
    {
        public int PatientId { get; set; }
        public string PatientName { get; set; }
        public string Reason { get; set; }
        public DateTime AppointmentDate { get; set; }
    }
}
