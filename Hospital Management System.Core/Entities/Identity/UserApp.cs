using Microsoft.AspNetCore.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hospital_Management_System.Core.Entities.Identity
{
    public class UserApp:IdentityUser
    {
        public string FName { get; set; }
        public string LName { get; set; }
        public string? country { get; set; }
        public string? city { get; set; }
        public string? Gender { get; set; }
        public DateOnly DateOfBirth { get; set; }
        public string? ProfileImageUrl { get; set; }


    }
}
