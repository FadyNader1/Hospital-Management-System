namespace Hospital_Management_System.DTOs
{
    public class PatientMedicalReportsDTO
    {
        public int DoctorId { get; set; }
        public string DoctorName { get; set; }
        public string Diagnosis { get; set; }
        public string Treatment { get; set; }
        public DateTime VisitDate { get; set; }
    }
}
