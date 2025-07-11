using AutoMapper;
using Hospital_Management_System.Core.Entities;
using Hospital_Management_System.Core.Interfaces;
using Hospital_Management_System.DTOs;
using Hospital_Management_System.Errors;
using Hospital_Management_System.Repository.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore.Metadata;
using System.Diagnostics;

namespace Hospital_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MedicalReportController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public MedicalReportController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }
        [Authorize(Roles = "Doctor")]
        [HttpPost("AddMedicalReport")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiValidationError), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddMedicalReport(MedicalReportDTO medicalReportDTO)
        {
            if (medicalReportDTO == null)
                return BadRequest(new ApiValidationError()
                {
                    Errors = new List<string> { "Medical report data is invalid." }
                });
            var patientspec = new PatientSpecification(medicalReportDTO.DoctorId);
            var patient = await unitOfWork.Repository<Patient>().GetByIdSpecAsync(patientspec);
            var doctorspec = new DoctorSpecification(medicalReportDTO.DoctorId);
            var doctor = await unitOfWork.Repository<Doctor>().GetByIdSpecAsync(doctorspec);
            if (patient == null || doctor == null)
                return NotFound(new ApiValidationError()
                {
                    Errors = new List<string> { "Invalid Patient Or Doctor Id" }
                });

            var medicalreportmapp = mapper.Map<MedicalReportDTO, MedicalReport>(medicalReportDTO);
            try
            {
                await unitOfWork.Repository<MedicalReport>().AddAsync(medicalreportmapp);
                await unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiValidationError()
                {
                    Errors = new List<string> { "An unexpected error occurred. Please try again later." }
                });
            }
            var response = new MedicalReportDetailsDTO()
            {
                Id = medicalreportmapp.Id,
                Diagnosis = medicalreportmapp.Diagnosis,
                Treatment = medicalreportmapp.Treatment,
                VisitDate = medicalreportmapp.VisitDate,
                Patient = new PatientBasicDTO()
                {
                    Id = medicalreportmapp.Patient.Id,
                    Name = medicalreportmapp.Patient.Name,
                },
                Doctor = new DoctorBasicDTO()
                {
                    Id = medicalreportmapp.Doctor.Id,
                    Name = medicalreportmapp.Doctor.FName
                }
            };

            return Ok(new { message = "MedicalReport Added Successfully", MedicalReport = response });
        }

        [Authorize(Roles = "Doctor,Staff")]
        [HttpGet("GetAllMedicalReport")]
        [ProducesResponseType(typeof(ApiHandleError), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<MedicalReportDetailsDTO>>> GetAllMedicalReport()
        {
            var spec = new MedicalReportSpecification();
            var medicalreports = await unitOfWork.Repository<MedicalReport>().GetAllWithSpecAsync(spec);
            if (medicalreports == null)
                return NotFound(new ApiValidationError()
                {
                    Errors = new List<string> { "Not MedicalReport Found" }
                });
            var resolnselist = new List<MedicalReportDetailsDTO>();
            foreach (var x in medicalreports)
            {
                var response = new MedicalReportDetailsDTO()
                {
                    Id = x.Id,
                    Diagnosis = x.Diagnosis,
                    Treatment = x.Treatment,
                    VisitDate = x.VisitDate,
                    Doctor = new DoctorBasicDTO()
                    {
                        Id = x.Doctor.Id,
                        Name = x.Doctor.FName
                    },
                    Patient = new PatientBasicDTO()
                    {
                        Id = x.Patient.Id,
                        Name = x.Patient.Name
                    }
                };
                resolnselist.Add(response);
            }
            return Ok(resolnselist);
        }
        [Authorize(Roles = "Doctor,Staff")]
        [HttpGet("GetMedicalReportById/{id}")]
        [ProducesResponseType(typeof(ApiHandleError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiHandleError), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<MedicalReportDetailsDTO>> GetMedicalReportById(int id)
        {
            var spec = new MedicalReportSpecification(id);
            var medicalreport = await unitOfWork.Repository<MedicalReport>().GetByIdSpecAsync(spec);
            if (medicalreport == null)
                return NotFound(new ApiValidationError()
                {
                    Errors = new List<string> { "Invalid MedicalReport Id" }
                });
            var response = new MedicalReportDetailsDTO()
            {
                Id = medicalreport.Id,
                Diagnosis = medicalreport.Diagnosis,
                Treatment = medicalreport.Treatment,
                VisitDate = medicalreport.VisitDate,
                Doctor = new DoctorBasicDTO()
                {
                    Id = medicalreport.Doctor.Id,
                    Name = medicalreport.Doctor.FName
                },
                Patient = new PatientBasicDTO()
                {
                    Id = medicalreport.Patient.Id,
                    Name = medicalreport.Patient.Name
                }
            };
            return Ok(response);
        }
        [Authorize(Roles = "Doctor")]
        [HttpPut("UpdateMedicalReport/{id}")]
        [ProducesResponseType(typeof(ApiHandleError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiHandleError), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> UpdateMedicalReport(int id, MedicalReportDTO medicalReportDTO)
        {
            var spec = new MedicalReportSpecification(id);
            var medicalreport = await unitOfWork.Repository<MedicalReport>().GetByIdSpecAsync(spec);
            if (medicalreport == null)
                return NotFound(new ApiValidationError()
                {
                    Errors = new List<string> { "Invalid MedicalReport Id" }
                });
            if (medicalreport == null)
                return NotFound(new ApiValidationError()
                {
                    Errors = new List<string> { "Invalid MedicalReport Data." }
                });
            medicalreport.Diagnosis = medicalReportDTO.Diagnosis;
            medicalreport.Treatment = medicalReportDTO.Treatment;
            medicalreport.VisitDate = medicalReportDTO.VisitDate;
            medicalreport.DoctorId = medicalReportDTO.DoctorId;
            medicalreport.PatientId = medicalReportDTO.PatientId;
            try
            {
                unitOfWork.Repository<MedicalReport>().Update(medicalreport);
                await unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiValidationError()
                {
                    Errors = new List<string> { "An unexpected error occurred. Please try again later." }
                });
            }
            var response = new MedicalReportDetailsDTO()
            {
                Id = medicalreport.Id,
                Diagnosis = medicalreport.Diagnosis,
                Treatment = medicalreport.Treatment,
                VisitDate = medicalreport.VisitDate,
                Doctor = new DoctorBasicDTO()
                {
                    Id = medicalreport.Doctor.Id,
                    Name = medicalreport.Doctor.FName
                },
                Patient = new PatientBasicDTO()
                {
                    Id = medicalreport.Patient.Id,
                    Name = medicalreport.Patient.Name
                }
            };
            return Ok(new { message = "MedicalReport Updated Successfully.", MedicalReport = response });
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteMedicalReport/{id}")]
        [ProducesResponseType(typeof(ApiHandleError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(ApiHandleError), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> DeleteMedicalReport(int id)
        {
            var spec = new MedicalReportSpecification(id);
            var medicalreport = await unitOfWork.Repository<MedicalReport>().GetByIdSpecAsync(spec);
            if (medicalreport == null)
                return NotFound(new ApiValidationError()
                {
                    Errors = new List<string> { "Invalid MedicalReport Id" }
                });
            try
            {
                unitOfWork.Repository<MedicalReport>().Delete(medicalreport);
                await unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiValidationError()
                {
                    Errors = new List<string> { "An unexpected error occurred. Please try again later." }
                });
            }
            return Ok(new { message = "MedicalReport Deleted Successfi=ully." });
        }



    }
}
