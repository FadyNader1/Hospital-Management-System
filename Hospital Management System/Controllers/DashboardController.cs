using Hospital_Management_System.Core.Interfaces;
using Hospital_Management_System.DTOs;
using Hospital_Management_System.Repository.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize(Roles = "Admin")]
    public class DashboardController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;

        public DashboardController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        [HttpGet("stats")]
        public async Task<ActionResult<StatsDTO>> GetStats()
        {
            // This is a placeholder for the actual implementation of stats retrieval
            // You would typically use the unitOfWork to get counts of patients, doctors, and appointments

            var spec1 = new PatientSpecification();
            var patients =await unitOfWork.Repository<Core.Entities.Patient>().CountAsync(spec1);
            var spec2 = new DoctorSpecification(null);
            var doctors = await unitOfWork.Repository<Core.Entities.Doctor>().CountAsync(spec2);
            var spec3 = new AppointmentSpecification();
            var appointments = await unitOfWork.Repository<Core.Entities.Appointment>().CountAsync(spec3);
            var spec4 = new MedicalReportSpecification();
            var medicalReports = await unitOfWork.Repository<Core.Entities.MedicalReport>().CountAsync(spec4);
            var result = new StatsDTO
            {
                TotalPatients = patients,
                TotalDoctors = doctors,
                TotalAppointments = appointments,
                TotalMedicalReports= medicalReports
            };
            if (result == null)
            {
                return NotFound("No stats found");
            }
            return Ok(result);

        }
    }
}
