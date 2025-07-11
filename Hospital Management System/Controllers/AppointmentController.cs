using AutoMapper;
using Hospital_Management_System.Core.Entities;
using Hospital_Management_System.Core.Interfaces;
using Hospital_Management_System.DTOs;
using Hospital_Management_System.Errors;
using Hospital_Management_System.Repository.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;

namespace Hospital_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class AppointmentController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;

        public AppointmentController(IUnitOfWork unitOfWork, IMapper mapper)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
        }

        [Authorize(Roles = "Patient")]
        [HttpPost("AddAppointment")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiValidationError), StatusCodes.Status400BadRequest)]
        public async Task<ActionResult> AddAppointment(AppointmentDTO appointmentDTO)
        {
            if (appointmentDTO == null)
                return BadRequest(new ApiValidationError()
                {
                    Errors = new List<string>() { "Appointment Data Invalid" }
                });
            var patientspec = new PatientSpecification(appointmentDTO.PatientId);
            var patient = await unitOfWork.Repository<Patient>().GetByIdSpecAsync(patientspec);
            var doctorspec = new DoctorSpecification(appointmentDTO.DoctorId);
            var doctor = await unitOfWork.Repository<Doctor>().GetByIdSpecAsync(doctorspec);
            // Check if the patient and doctor exist
            if (patient == null || doctor == null)
                return BadRequest(new ApiValidationError()
                {
                    Errors = new List<string> { "Invalid Patient or Doctor ID" }
                });
            var appointmentmapp = mapper.Map<AppointmentDTO, Appointment>(appointmentDTO);


            try
            {
                await unitOfWork.Repository<Appointment>().AddAsync(appointmentmapp);
                await unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiValidationError()
                {
                    Errors = new List<string> { "An unexpected error occurred. Please try again later." }
                });
            }


            var response = new AppointmentDetailsDTO()
            {
                id = appointmentmapp.Id,
                Reason = appointmentmapp.Reason,
                AppointmentDate = appointmentmapp.AppointmentDate,
                Patient = new PatientBasicDTO()
                {
                    Id = patient.Id,
                    Name = patient.Name,
                },
                Doctor = new DoctorBasicDTO()
                {
                    Id = doctor.Id,
                    Name = doctor.FName,
                }
            };
            return Ok(new { message = "Appointment Added Successfully ", Appointment = response });
        }

        [Authorize(Roles = "Patient")]
        [HttpGet("GetAllAppointments")]
        [ProducesResponseType(typeof(IEnumerable<AppointmentDetailsDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiHandleError), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<AppointmentDetailsDTO>>> GetAllAppointments()
        {
            var spec = new AppointmentSpecification();
            var appointments = await unitOfWork.Repository<Appointment>().GetAllWithSpecAsync(spec);
            if (appointments == null || !appointments.Any())
                return NotFound(new ApiHandleError(404));

            var resultlist = new List<AppointmentDetailsDTO>();
            foreach (var y in appointments)
            {
                var result = new AppointmentDetailsDTO()
                {
                    id = y.Id,
                    Reason = y.Reason,
                    AppointmentDate = y.AppointmentDate,
                    Patient = new PatientBasicDTO()
                    {
                        Id = y.PatientId,
                        Name = y.Patient.Name
                    },
                    Doctor = new DoctorBasicDTO()
                    {
                        Id = y.DoctorId,
                        Name = y.Doctor.FName
                    },
                    Status = y.Status
                };
                resultlist.Add(result);
            }
            return Ok(resultlist);
        }
        [Authorize(Roles = "Admin, Doctor, Patient")]
        [HttpGet("GetAppointmentById/{id}")]
        [ProducesResponseType(typeof(ApiHandleError), StatusCodes.Status404NotFound)]
        [ProducesResponseType(typeof(AppointmentDetailsDTO), StatusCodes.Status200OK)]
        public async Task<ActionResult<AppointmentDetailsDTO>> GetAppointmentById(int id)
        {
            var spec = new AppointmentSpecification(id);
            var appointment = await unitOfWork.Repository<Appointment>().GetByIdSpecAsync(spec);
            if (appointment == null)
                return BadRequest(new ApiValidationError()
                {
                    Errors = new List<string> { "Invalid Appointment ID" }
                });
            var result = new AppointmentDetailsDTO()
            {
                id = appointment.Id,
                Reason = appointment.Reason,
                AppointmentDate = appointment.AppointmentDate,
                Patient = new PatientBasicDTO()
                {
                    Id = appointment.PatientId,
                    Name = appointment.Patient.Name
                },
                Doctor = new DoctorBasicDTO()
                {
                    Id = appointment.DoctorId,
                    Name = appointment.Doctor.FName
                },
                Status = appointment.Status

            };
            return Ok(result);
        }
        [Authorize(Roles = "Patient")]
        [HttpPut("UpdateAppointment/{id}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiValidationError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiValidationError), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<string>> UpdateAppointment(int id, AppointmentDTO appointmentDTO)
        {
            if (appointmentDTO == null)
                return BadRequest(new ApiValidationError()
                {
                    Errors = new List<string> { "Invalid Appointment Dats" }
                });
            var sepc = new AppointmentSpecification(id);
            var appointment = await unitOfWork.Repository<Appointment>().GetByIdSpecAsync(sepc);
            if (appointment == null)
                return BadRequest(new ApiValidationError()
                {
                    Errors = new List<string> { "Invalid ID" }
                });
            var patientspec = new PatientSpecification(appointmentDTO.PatientId);
            var patient = await unitOfWork.Repository<Patient>().GetByIdSpecAsync(patientspec);
            var doctorspec = new DoctorSpecification(appointmentDTO.DoctorId);
            var doctor = await unitOfWork.Repository<Doctor>().GetByIdSpecAsync(doctorspec);
            // Check if the patient and doctor exist
            if (patient == null || doctor == null)
                return BadRequest(new ApiValidationError()
                {
                    Errors = new List<string> { "Invalid Patient or Doctor ID" }
                });
            appointment.Reason = appointmentDTO.Reason;
            appointment.AppointmentDate = appointmentDTO.AppointmentDate;
            appointment.DoctorId = appointmentDTO.DoctorId;
            appointment.PatientId = appointmentDTO.PatientId;

            try
            {
                unitOfWork.Repository<Appointment>().Update(appointment);
                await unitOfWork.Complete();
            }
            catch
            {
                return BadRequest(new ApiValidationError()
                {
                    Errors = new List<string> { "An unexpected error occurred. Please try again later." }
                });
            }
            var response = new AppointmentDetailsDTO()
            {
                id = appointment.Id,
                Reason = appointment.Reason,
                AppointmentDate = appointment.AppointmentDate,
                Patient = new PatientBasicDTO()
                {
                    Id = appointment.PatientId,
                    Name = appointment.Patient.Name
                },
                Doctor = new DoctorBasicDTO()
                {
                    Id = appointment.DoctorId,
                    Name = appointment.Doctor.FName
                },
                Status = appointment.Status
            };
            return Ok(new { Message = "Appointment Update Successfully", Appointment = response });
        }

        [Authorize(Roles = "Patient")]
        [HttpDelete("DeleteAppointment/{id}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiValidationError), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<string>> DeleteAppointment(int id)
        {
            var spec = new AppointmentSpecification(id);
            var appointment = await unitOfWork.Repository<Appointment>().GetByIdSpecAsync(spec);
            if (appointment == null)
                return NotFound(new ApiValidationError()
                {
                    Errors = new List<string> { "Invalid Appointment ID" }
                });
            try
            {
                unitOfWork.Repository<Appointment>().Delete(appointment);
                await unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiValidationError()
                {
                    Errors = new List<string> { "An unexpected error occurred. Please try again later." }
                });
            }
            return Ok(new { Message = "Appointment Deleted Successfully" });
        }

        [Authorize(Roles = "Doctor,Staff")]
        [HttpPost("ApproveAppointment/{id}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiValidationError), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<string>> ApproveAppointment(int id)
        {
            var spec = new AppointmentSpecification(id);
            var appointment = await unitOfWork.Repository<Appointment>().GetByIdSpecAsync(spec);
            if (appointment == null)
                return NotFound(new ApiValidationError()
                {
                    Errors = new List<string> { " Invalid Appointment ID" }
                });
            try
            {
                appointment.Status = AppointmentStatus.Approved;
                unitOfWork.Repository<Appointment>().Update(appointment);
                await unitOfWork.Complete();
            }
            catch
            {
                return BadRequest(new ApiValidationError()
                {
                    Errors = new List<string> { "An unexpected error occurred. Please try again later." }
                });
            }
            var response = new AppointmentDetailsDTO()
            {
                id = appointment.Id,
                Reason = appointment.Reason,
                AppointmentDate = appointment.AppointmentDate,
                Patient = new PatientBasicDTO()
                {
                    Id = appointment.PatientId,
                    Name = appointment.Patient.Name
                },
                Doctor = new DoctorBasicDTO()
                {
                    Id = appointment.DoctorId,
                    Name = appointment.Doctor.FName
                },
                Status = appointment.Status
            };
            return Ok(new { Message = "Appointment Approved Successfully", Appointment = response });
        }

        [Authorize(Roles = "Doctor,Staff")]
        [HttpPost("RejectAppointment/{id}")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiValidationError), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> RejectAppointment(int id)
        {
            var spec = new AppointmentSpecification(id);
            var appointment = await unitOfWork.Repository<Appointment>().GetByIdSpecAsync(spec);
            if (appointment == null)
                return NotFound(new ApiValidationError()
                {
                    Errors = new List<string> { " Invalid Appointment ID" }
                });
            try
            {
                appointment.Status = AppointmentStatus.Rejected;
                unitOfWork.Repository<Appointment>().Update(appointment);
                await unitOfWork.Complete();
            }
            catch
            {
                return BadRequest(new ApiValidationError()
                {
                    Errors = new List<string> { "An unexpected error occurred. Please try again later." }
                });
            }
            var response = new AppointmentDetailsDTO()
            {
                id = appointment.Id,
                Reason = appointment.Reason,
                AppointmentDate = appointment.AppointmentDate,
                Patient = new PatientBasicDTO()
                {
                    Id = appointment.PatientId,
                    Name = appointment.Patient.Name
                },
                Doctor = new DoctorBasicDTO()
                {
                    Id = appointment.DoctorId,
                    Name = appointment.Doctor.FName
                },
                Status = appointment.Status
            };
            return Ok(new { Message = "Appointment Rejected Successfully", Appointment = response });
        }

    }
}