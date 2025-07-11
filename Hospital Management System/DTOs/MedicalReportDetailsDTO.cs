namespace Hospital_Management_System.DTOs
{
    public class MedicalReportDetailsDTO
    {
        public int Id { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public DateTime VisitDate { get; set; }
        public PatientBasicDTO Patient { get; set; }
        public DoctorBasicDTO Doctor { get; set; }
    }
}
