using Hospital_Management_System.Core.Entities;

namespace Hospital_Management_System.DTOs
{
    public class MedicalReportDTO
    {
        public int PatientId { get; set; }
        public int DoctorId { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public DateTime VisitDate { get; set; }
    }
}
