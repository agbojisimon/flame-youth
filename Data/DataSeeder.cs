using GlobalFlameMinistry.API.Models;
using Microsoft.AspNetCore.Identity;

namespace GlobalFlameMinistry.API.Data
{
    public static class DataSeeder
    {
        public static async Task SeedAdminAsync(
            UserManager<AppUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            string[] roles = ["Admin", "Member", "YouthMember"];

            foreach (var role in roles)
            {
                // Only create the role if it doesn't already exist
                if (!await roleManager.RoleExistsAsync(role))
                {
                    await roleManager.CreateAsync(new IdentityRole(role));
                }
            }

            var mentor = "agbojisimon107@gmail.com";
            var existingAdmin = await userManager.FindByEmailAsync(mentor);

            if (existingAdmin is null)
            {
                var admin = new AppUser
                {
                    FirstName = "Simon",
                    LastName = "Admin",
                    UserName = mentor,
                    Email = mentor,
                    EmailConfirmed = true,
                    CreatedOn = DateTime.UtcNow
                };

                var result = await userManager.CreateAsync(admin, "Mentor22.");

                if (result.Succeeded)
                {
                    // Assign Admin role to this user
                    await userManager.AddToRoleAsync(admin, "Admin");
                }
                else
                {
                    var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                    throw new Exception($"Failed to seed admin user: {errors}");
                }
            }
        }
    }
}