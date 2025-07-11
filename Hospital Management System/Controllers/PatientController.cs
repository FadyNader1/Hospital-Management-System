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
using System.Security.Claims;

namespace Hospital_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class PatientController : ControllerBase
    {
        private readonly IUnitOfWork unitOfWork;
        private readonly IMapper mapper;
        private readonly UserManager<UserApp> userManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ITokenServices tokenServices;

        public PatientController(IUnitOfWork unitOfWork, IMapper mapper, UserManager<UserApp> userManager, RoleManager<IdentityRole> roleManager,ITokenServices tokenServices)
        {
            this.unitOfWork = unitOfWork;
            this.mapper = mapper;
            this.userManager = userManager;
            this.roleManager = roleManager;
            this.tokenServices = tokenServices;
        }

        [HttpPost("AddPatient")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiHandleError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiValidationError), StatusCodes.Status400BadRequest)]
        [Authorize(Roles = "User,Admin,Staff")]
        public async Task<IActionResult> AddPatient([FromBody] PatientTopDTO patientTopDTO)
        {
           
            var email = User.FindFirstValue(ClaimTypes.Email);
            var user = await userManager.FindByEmailAsync(email);
            if (user == null)
                return BadRequest(new ApiValidationError()
                {
                    Errors = new List<string> { "User not found. Please register first." }
                });
           

            if (await userManager.IsInRoleAsync(user, "Patient"))
                return BadRequest(new ApiValidationError()
                {
                    Errors = new List<string> { "You are already registered as a Patient." }
                });

            if (await userManager.IsInRoleAsync(user, "Doctor"))
                return BadRequest(new ApiValidationError()
                {
                    Errors = new List<string> { "You cannot register as a Patient while being a Doctor." }
                });

            if (await userManager.IsInRoleAsync(user, "User"))
            {
                var patientmapp = mapper.Map<PatientDTO, Patient>(patientTopDTO.patientDTO);
                try
                {
                    await unitOfWork.Repository<Patient>().AddAsync(patientmapp);
                    await unitOfWork.Complete();
                }
                catch (Exception ex)
                {
                    return BadRequest(new ApiValidationError()
                    {
                        Errors = new List<string> { ex.Message },
                    });
                }
                if (!await roleManager.RoleExistsAsync("Patient"))
                    await roleManager.CreateAsync(new IdentityRole("Patient"));
                if (!await userManager.IsInRoleAsync(user, "Patient"))
                    await userManager.AddToRoleAsync(user, "Patient");
                await userManager.RemoveFromRoleAsync(user, "User");

                return Ok(new
                {
                    message = $"Successfully Added {patientmapp.Name}",
                    patient =patientTopDTO.patientDTO,
                    role = await userManager.GetRolesAsync(user)
                });
            }

            if (await userManager.IsInRoleAsync(user, "Admin") || await userManager.IsInRoleAsync(user, "Staff"))
            {
                if (patientTopDTO.newPatientDTO == null)
                    return BadRequest(new ApiValidationError()
                    {
                        Errors = new List<string> { "NewPatientDTO cannot be null" }
                    });
                var checkuser = await userManager.FindByEmailAsync(patientTopDTO.newPatientDTO.Email);
                if (checkuser != null)
                    return BadRequest(new ApiValidationError()
                    {
                        Errors = new List<string> { "Email already exists. Please use a different email." }
                    });



                var newUser = new UserApp()
                {
                    FName =patientTopDTO.newPatientDTO.FName,
                    LName = patientTopDTO.newPatientDTO.LName,
                    Email = patientTopDTO.newPatientDTO.Email,
                    UserName = patientTopDTO.newPatientDTO.Email,
                    DateOfBirth = patientTopDTO.newPatientDTO.DateOfBirth,
                    ProfileImageUrl = patientTopDTO.newPatientDTO.ProfileImageUrl,
                    country = patientTopDTO.newPatientDTO.country,
                    city = patientTopDTO.newPatientDTO.city,

                };
                var result = await userManager.CreateAsync(newUser, patientTopDTO.newPatientDTO.Password);
                if (!result.Succeeded)
                {
                    return BadRequest(new ApiValidationError()
                    {
                        Errors = result.Errors.Select(e => e.Description).ToList()
                    });
                }
                if (!await roleManager.RoleExistsAsync("Patient"))
                    await roleManager.CreateAsync(new IdentityRole("Patient"));
                if (!await userManager.IsInRoleAsync(newUser, "Patient"))
                    await userManager.AddToRoleAsync(newUser, "Patient");
                if (patientTopDTO.patientDTO == null)
                    return BadRequest(new ApiHandleError(400));
                var patientmapp = mapper.Map<PatientDTO, Patient>(patientTopDTO.patientDTO);
                try
                {
                    await unitOfWork.Repository<Patient>().AddAsync(patientmapp);
                    await unitOfWork.Complete();
                }
                catch (Exception ex)
                {
                    return BadRequest(new ApiValidationError()
                    {
                        Errors = new List<string> { ex.Message },
                    });
                }
                return Ok(new
                {
                    message = $"Successfully Added {patientmapp.Name}",
                    User =new
                    {
                        FirstName = patientTopDTO.newPatientDTO.FName,
                        LastName = patientTopDTO.newPatientDTO.LName,
                        Email = patientTopDTO.newPatientDTO.Email,
                        Date= patientTopDTO.newPatientDTO.DateOfBirth.ToString("yyyy-MM-dd"),
                        Token = await tokenServices.CreateTokenAsync(newUser, userManager)
                    },
                    patientdata = mapper.Map<Patient, PatientDTO>(patientmapp),
                    role = await userManager.GetRolesAsync(newUser)
                });
            }
            return BadRequest(new ApiValidationError()
            {
                Errors = new List<string> { "You are not authorized to add a Patient." }
            });
        }

        [Authorize(Roles = "Admin,Staff")]
        [HttpGet("GetAllPatients")]
        [ProducesResponseType(typeof(IEnumerable<PatientDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiHandleError), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<PatientDTO>>> GetAllPatients()
        {
            var spec = new PatientSpecification();
            var patients = await unitOfWork.Repository<Patient>().GetAllWithSpecAsync(spec);
            if (patients == null || !patients.Any())
                return NotFound(new ApiHandleError(404));
            var patientsDTO = mapper.Map<IReadOnlyList<Patient>, IReadOnlyList<PatientDTO>>(patients);
            if (patientsDTO == null || !patientsDTO.Any())
                return NotFound(new ApiHandleError(404, "No patients found."));



            return Ok(patientsDTO);
        }
        [Authorize(Roles = "Admin,Staff,Doctor")]
        [HttpGet("GetPatientById")]
        [ProducesResponseType(typeof(PatientDTO), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiHandleError), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<PatientDTO>> GetPatientById(int id)
        {
            var spec = new PatientSpecification(id);
            var patient = await unitOfWork.Repository<Patient>().GetByIdSpecAsync(spec);
            if (patient == null)
                return NotFound(new ApiValidationError()
                {
                    Errors = new List<string> { "Patient not found" }
                });
            var patientDTO = mapper.Map<Patient, PatientDTO>(patient);

            return Ok(patientDTO);
        }
        [Authorize(Roles = "Admin,Staff")]
        [HttpPut("UpdatePatient")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiHandleError), StatusCodes.Status400BadRequest)]
        [ProducesResponseType(typeof(ApiHandleError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> UpdatePatient(int id, [FromBody] PatientDTO patientDTO)
        {
            if (patientDTO is null)
                return BadRequest(new ApiHandleError(400));
            var patient = await unitOfWork.Repository<Patient>().GetByIdAsync(id);
            if (patient == null)
                return NotFound(new ApiValidationError()
                {
                    Errors = new List<string> { "Patient not found" }
                });

            patient.Name = patientDTO.Name;
            patient.Age = patientDTO.Age;
            patient.PhoneNumber = patientDTO.PhoneNumber;
            patient.ChronicDiseases = patientDTO.ChronicDiseases;
            try
            {
                unitOfWork.Repository<Patient>().Update(patient);
                await unitOfWork.Complete();
            }
            catch (Exception ex)
            {
                return BadRequest(new ApiValidationError()
                {
                    Errors = new List<string> { "An unexpected error occurred. Please try again later." }
                });
            }
            return Ok(new
            {
                message = $"Successfully Updated {patient.Name}",
                patient = mapper.Map<PatientDTO>(patient)
            });
        }
        [Authorize(Roles = "Admin")]
        [HttpDelete("DeletePatient")]
        [ProducesResponseType(typeof(string), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiHandleError), StatusCodes.Status404NotFound)]
        public async Task<IActionResult> DeletePatient(int id)
        {
            var patient = await unitOfWork.Repository<Patient>().GetByIdAsync(id);
            if (patient == null)
                return NotFound(new ApiValidationError()
                {
                    Errors = new List<string> { "Patient not found" }
                });
            try
            {
                unitOfWork.Repository<Patient>().Delete(patient);
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
                message = $"Successfully Deleted {patient.Name}",
                patient = mapper.Map<Patient, PatientDTO>(patient)
            });
        }
        [Authorize(Roles = "Admin,Staff,Doctor,Patient")]
        [HttpGet("PatientMedicalReports/{id}")]
        [ProducesResponseType(typeof(List<PatientMedicalReportsDTO>), StatusCodes.Status200OK)]
        [ProducesResponseType(typeof(ApiHandleError), StatusCodes.Status404NotFound)]
        public async Task<ActionResult<IReadOnlyList<PatientMedicalReportsDTO>>> PatientMedicalReports(int id)
        {
            var spec = new PatientSpecification(id);
            var patient = await unitOfWork.Repository<Patient>().GetByIdSpecAsync(spec);
            if (patient == null)
                return NotFound(new ApiValidationError()
                {
                    Errors = new List<string> { "Patient Not Found" }
                });

            var resultlist = new List<PatientMedicalReportsDTO>();
            foreach (var x in patient.MedicalReports)
            {
                var result = new PatientMedicalReportsDTO()
                {
                    DoctorId = x.DoctorId,
                    DoctorName = x.Doctor.FName,
                    Diagnosis = x.Diagnosis,
                    Treatment = x.Treatment,
                    VisitDate = x.VisitDate
                };
                resultlist.Add(result);
            }
            if (resultlist == null && resultlist.Count == 0)
                return NotFound(new ApiValidationError()
                {
                    Errors = new List<string> { "No MedicalReport found for this Patient" }
                });
            return Ok(resultlist);
        }
    }
}