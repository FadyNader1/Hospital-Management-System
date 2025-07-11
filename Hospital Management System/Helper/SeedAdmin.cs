using Hospital_Management_System.Core.Entities.Identity;
using Microsoft.AspNetCore.Identity;

namespace Hospital_Management_System.Helper
{
    public class SeedAdmin
    {
        public static async Task AddAdmin(UserManager<UserApp> userManager, RoleManager<IdentityRole> roleManager)
        {
            if (!await roleManager.RoleExistsAsync("Admin"))
                await roleManager.CreateAsync(new IdentityRole("Admin"));

            var checkuser = await userManager.FindByEmailAsync("admin123@gmail.com");
            if (checkuser == null)
            {
                var user = new UserApp()
                {
                    //Id = Guid.NewGuid().ToString(),
                    FName = "Super",
                    LName = "Admin",
                    Email = "admin123@gmail.com",
                    UserName = "Super_Admin",
                };
                await userManager.CreateAsync(user, "P@ssw0rd");
                if (!await userManager.IsInRoleAsync(user, "Admin"))
                    await userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}