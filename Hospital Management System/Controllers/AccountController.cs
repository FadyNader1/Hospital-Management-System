using AutoMapper;
using Hospital_Management_System.Core.Entities.Identity;
using Hospital_Management_System.Core.Interfaces;
using Hospital_Management_System.DTOs.IdentityDTO;
using Hospital_Management_System.Errors;
using Hospital_Management_System.Helper;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using static System.Net.Mime.MediaTypeNames;
using static System.Runtime.InteropServices.JavaScript.JSType;

namespace Hospital_Management_System.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly UserManager<UserApp> userManager;
        private readonly SignInManager<UserApp> signInManager;
        private readonly RoleManager<IdentityRole> roleManager;
        private readonly ImageSettings imageSettings;
        private readonly IMapper mapper;
        private readonly ITokenServices tokenServices;

        public AccountController(UserManager<UserApp> userManager, SignInManager<UserApp> signInManager, RoleManager<IdentityRole> roleManager, ImageSettings imageSettings, IMapper mapper, ITokenServices tokenServices)
        {
            this.userManager = userManager;
            this.signInManager = signInManager;
            this.roleManager = roleManager;
            this.imageSettings = imageSettings;
            this.mapper = mapper;
            this.tokenServices = tokenServices;
        }

        [HttpPost("Register")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> Register([FromForm] RegisterDTO registerDTO)
        {
            if (registerDTO == null)
                return BadRequest(new ApiValidationError()
                {
                    Errors = new[] { "Invalid registration data." }
                });
            var checkuser = await userManager.FindByEmailAsync(registerDTO.Email);
            if (checkuser != null)
            {
                return BadRequest(new ApiValidationError()
                {
                    Errors = new[] { "Email already exists." }
                });
            }
            var user = new UserApp()
            {
                FName = registerDTO.FirstName,
                LName = registerDTO.LastName,
                country = registerDTO.Country,
                city = registerDTO.City,
                DateOfBirth = registerDTO.DateOfBirth,
                Email = registerDTO.Email,
                PhoneNumber = registerDTO.PhoneNumber,
                UserName = $"{registerDTO.FirstName}_{registerDTO.LastName}",
                Gender = registerDTO.Gender,
                ProfileImageUrl = await imageSettings.UploadImage(registerDTO.ProfileImage) ?? null // Handle the case where the image upload fails

            };
           
            var result = await userManager.CreateAsync(user, registerDTO.Password);
            if (!result.Succeeded)
            {
                foreach (var error in result.Errors)
                {
                    return BadRequest(new ApiValidationError()
                    {
                        Errors = new[] { error.Description }
                    });
                }
            }
            var checkRole = await roleManager.RoleExistsAsync("User");
            if (!checkRole)
            {
                var role = new IdentityRole("User");
                await roleManager.CreateAsync(role);
            }
            await userManager.AddToRoleAsync(user, "User");

            var response = mapper.Map<UserResponseDTO>(user);
            return Ok(new { messag = "Register Successfully", model = response, Token = await tokenServices.CreateTokenAsync(user, userManager) ,role="User" });
        }

        [HttpPost("Login")]
        [ProducesResponseType(StatusCodes.Status200OK)]
        [ProducesResponseType(StatusCodes.Status400BadRequest)]
        public async Task<ActionResult<string>> Login(LoginDTO loginDTO)
        {

            var user = await userManager.FindByEmailAsync(loginDTO.Email);
            if (user == null)
            {
                return BadRequest(new ApiValidationError()
                {
                    Errors = new List<string> { "Invalid email, Email not found." }
                });
            }
            var checkpassword = await userManager.CheckPasswordAsync(user, loginDTO.Password);
            if (!checkpassword)
            {
                return BadRequest(new ApiValidationError()
                {
                    Errors = new List<string> { "Invalid password." }
                });
            }
            var result = await signInManager.PasswordSignInAsync(user, loginDTO.Password, true, false);
            if (!result.Succeeded)
            {
                return BadRequest(new ApiValidationError()
                {
                    Errors = new List<string> { "Login failed." }
                });
            }
            var response = mapper.Map<UserResponseDTO>(user);
            return Ok(new { message = "Login Successfully", model = response, Token = await tokenServices.CreateTokenAsync(user, userManager) ,role=await userManager.GetRolesAsync(user) });
        }

    }
}
