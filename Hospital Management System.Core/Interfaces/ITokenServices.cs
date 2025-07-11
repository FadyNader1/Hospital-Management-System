using Hospital_Management_System.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital_Management_System.Core.Interfaces
{
    public interface ITokenServices
    {
        Task<string> CreateTokenAsync(UserApp user, UserManager<UserApp> userManager);
    }
}
