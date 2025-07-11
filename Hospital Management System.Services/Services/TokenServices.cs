using Hospital_Management_System.Core.Entities.Identity;
using Hospital_Management_System.Core.Interfaces;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System;
using System.Collections.Generic;
using System.IdentityModel.Tokens.Jwt;
using System.Linq;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace Hospital_Management_System.Services.Services
{
    public class TokenServices : ITokenServices
    {
        private readonly IConfiguration confg;

        public TokenServices(IConfiguration confg)
        {
            this.confg = confg;
        }
        public async Task<string> CreateTokenAsync(UserApp user, UserManager<UserApp> userManager)
        {
            var authckaims = new List<Claim>()
            {
                new Claim(ClaimTypes.GivenName,user.FName)
            };
            if (!string.IsNullOrEmpty(user.Email))
                authckaims.Add(new Claim(ClaimTypes.Email, user.Email));


            var userRoles=await userManager.GetRolesAsync(user);
            foreach (var role in userRoles)
            {
                authckaims.Add(new Claim(ClaimTypes.Role,role));
            }

            var authkey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(confg["JWT:Key"]));

            var token=new JwtSecurityToken(
                issuer: confg["JWT:Issuer"],
                audience: confg["JWT:Audience"],
                claims: authckaims,
                expires: DateTime.Now.AddMinutes(30),
                signingCredentials: new SigningCredentials(authkey, SecurityAlgorithms.HmacSha256)
            );

            return new JwtSecurityTokenHandler().WriteToken(token);

        }
    }
}
