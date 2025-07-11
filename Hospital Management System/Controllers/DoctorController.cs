using AutoMapper;
using Hospital_Management_System.Core.Entities;
using Hospital_Management_System.Core.Entities.Identity;
using Hospital_Management_System.Core.Interfaces;
using Hospital_Management_System.DTOs;
using Hospital_Management_System.Errors;
using Hospital_Management_System.Repository.Specifications;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.OpenApi.Extensions;
using System.Security.Claims;

namespace Hospital_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class DoctorController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly UserManager<UserApp> userManager;
        private readonly RoleManager<IdentityRole> roleManager;

        public DoctorController(IUnitOfWork unitOfWork, IMapper mapper, UserManager<UserApp> userManager, RoleManager<IdentityRole> roleManager)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.userManager = userManager;
            this.roleManager = roleManager;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost("AddDoctor")]
        [ProducesResponseType(typeof(ApiValidationError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(object), StatusCodes.Status200OK)]

        public async Task<ActionResult<string>> AddDoctor([FromBody] DoctorDTO doctorDTO)
        {
            if (doctorDTO == null)
            {
                return BadRequest(new ApiValidationError()
                {
                    Errors = new List<string> { "Doctor data is null" }
                });
            }
            // Validate the doctorDTO object
            foreach (var x in doctorDTO.doctorAvailability)
            {
                if (!Enum.IsDefined(typeof(DayOfWeek), x.Day))
                {
                    return BadRequest(new ApiValidationError
                    {
                        Errors = new List<string> { "Invalid day value. Use 0 (Sunday) to 6 (Saturday)." }
                    });
                }
            }

            var newUser = new UserApp()
            {
                FName = doctorDTO.FName,
                LName = doctorDTO.LName,
                UserName = $"{doctorDTO.FName}{doctorDTO.LName}",
                Email = doctorDTO.Email,
                country = doctorDTO.country,
                city = doctorDTO.city,
                Gender= doctorDTO.Gender,
                DateOfBirth = doctorDTO.DateOfBirth,
                ProfileImageUrl= doctorDTO.ProfileImageUrl
            };
            var checkuser = await userManager.FindByEmailAsync(newUser.Email);
            if (checkuser != null)
            {
                return BadRequest(new ApiValidationError()
                {
                    Errors = new List<string> { "Email already exists" }
                });
            }
            var result = await userManager.CreateAsync(newUser, doctorDTO.Password);
            if (!result.Succeeded)
            {
                return BadRequest(new ApiValidationError()
                {
                    Errors = result.Errors.Select(e => e.Description).ToList()
                });
            }
            if (! await roleManager.RoleExistsAsync("Doctor"))
                await roleManager.CreateAsync(new IdentityRole("Doctor"));
            if(!await userManager.IsInRoleAsync(newUser, "Doctor"))
                await userManager.AddToRoleAsync(newUser, "Doctor");

            var doctor = mapper.Map<DoctorDTO, Doctor>(doctorDTO);
            doctor.UserId = newUser.Id;
            try
            {
                await unitOfWork.Repository<Doctor>().AddAsync(doctor);
                await unitOfWork.Complete();

            }
            catch (Exception ex)
            {
                return BadRequest(new ApiValidationError()
                {
                    Errors = new List<string> { ex.Message }
                });
            }
        
            return Ok(new
            {
                Message = "Doctor added successfully",
                Doctor = doctorDTO,
                role = await userManager.GetRolesAsync(newUser)
            });
        }

        [HttpGet("GetAllDoctors")]
        [ProducesResponseType(typeof(ApiValidationError), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<DoctorDTO>>> GetAllDoctors(string? SearchBySpecialization)
        {
            var spec = new DoctorSpecification(SearchBySpecialization);
            var doctors = await unitOfWork.Repository<Doctor>().GetAllWithSpecAsync(spec);

            if (doctors == null || !doctors.Any())
                return NotFound(new ApiValidationError()
                {
                    Errors = new List<string> { "Not Found Doctors" }
                });

            var doctorsmapp = mapper.Map<IReadOnlyList<Doctor>, IReadOnlyList<DoctorDTO>>(doctors);

            return Ok(doctorsmapp);
        }

        [HttpGet("GetDoctorById/{id}")]
        [ProducesResponseType(typeof(ApiValidationError), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<DoctorDTO>> GetDoctorById(int id)
        {
            var spec = new DoctorSpecification(id);
            var doctor = await unitOfWork.Repository<Doctor>().GetByIdSpecAsync(spec);
            if (doctor == null)
                return NotFound(new ApiValidationError()
                {
                    Errors = new List<string> { "Doctor not found" }
                });
            var doctorMap = mapper.Map<Doctor, DoctorDTO>(doctor);
            return Ok(doctorMap);
        }
        [Authorize(Roles = "Admin")]
        [HttpPut("UpdateDoctor/{id}")]
        [ProducesResponseType(typeof(ApiValidationError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiValidationError), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> UpdateDoctor(int id, [FromBody] DoctorDTO doctorDTO)
        {
            if (doctorDTO == null)
                return BadRequest(new ApiValidationError()
                {
                    Errors = new List<string> { "Doctor data is null" }
                });

            var spec = new DoctorSpecification(id);
            var doctor = await unitOfWork.Repository<Doctor>().GetByIdSpecAsync(spec);
            if (doctor == null)
                return NotFound(new ApiValidationError()
                {
                    Errors = new List<string> { "Doctor not found" }
                });

            doctor.FName = doctorDTO.FName;
            doctor.Specialization = doctorDTO.Specialization;
            doctor.DoctorAvailabilities.Clear();
            foreach (var item in doctorDTO.doctorAvailability)
            {
                doctor.DoctorAvailabilities.Add(new DoctorAvailability()
                {
                    Day = item.Day,
                    AvailableFrom = item.AvailableFrom,
                    AvailableTo = item.AvailableTo,
                });
            }
            try
            {
                unitOfWork.Repository<Doctor>().Update(doctor);
                await unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiValidationError()
                {
                    Errors = new List<string> { ex.Message }
                });
            }
            return Ok(new
            {
                message = $"Successfully Updated {doctor.FName}",
                Doctor = doctorDTO
            });
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("DeleteDoctor/{id}")]
        [ProducesResponseType(typeof(ApiValidationError), StatusCodes.Status404NotFound)]
        public async Task<ActionResult> DeleteDoctor(int id)
        {
            var spec = new DoctorSpecification(id);
            var doctor = await unitOfWork.Repository<Doctor>().GetByIdSpecAsync(spec);
            if (doctor == null)
                return NotFound(new ApiValidationError()
                {
                    Errors = new List<string> { "Doctor not found" }
                });
            try
            {
                unitOfWork.Repository<Doctor>().Delete(doctor);
                await unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiValidationError()
                {
                    Errors = new List<string> { ex.Message }
                });
            }
            return Ok(new
            {
                message = $"Successfully Deleted Doctor with ID: {doctor.Id} and Name: {doctor.FName}"
            });
        }

        [Authorize(Roles = "Doctor")]
        [HttpGet("DoctorAppointments/{id}")]
        [ProducesResponseType(typeof(ApiValidationError), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<AppointmentDetailsDTO>>> DoctorAppointments(int id)
        {
            var spec = new DoctorSpecification(id);
            var doctor = await unitOfWork.Repository<Doctor>().GetByIdSpecAsync(spec);
            if (doctor == null)
                return NotFound(new ApiValidationError()
                {
                    Errors = new List<string> { "Doctor not found" }
                });
            var resultlist = new List<DoctorAppointmentDetailsDTO>();
            foreach (var x in doctor.Appointments)
            {
                var result = new DoctorAppointmentDetailsDTO()
                {
                    PatientId = x.Patient.Id,
                    PatientName = x.Patient.Name,
                    Reason = x.Reason,
                    AppointmentDate = x.AppointmentDate


                };
                resultlist.Add(result);
            }
            if (resultlist.Count == 0)
                return NotFound(new ApiValidationError()
                {
                    Errors = new List<string> { "No appointments found for this doctor" }
                });
            return Ok(resultlist);
        }

    }
}