using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;

namespace IrisEye.Web.Utils
{
    public class DbInitializer
    {
        public static async Task SeedUsers(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            var roleName = Enum.GetNames(typeof(SystemRoles));

            foreach (var role in roleName)
            {
                var roleExists = await roleManager.RoleExistsAsync(role);
                if (!roleExists)
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var admin = await userManager.FindByNameAsync("admin");
            if (admin == null)
            {
                var identityUser = new IdentityUser("admin") {Email = "admin@iriseye.com"};
                var result = await userManager.CreateAsync(identityUser, "Administrator123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(identityUser, SystemRoles.Administrator.ToString());    
                }
            }
        }
    }

}