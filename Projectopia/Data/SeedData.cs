using Microsoft.AspNetCore.Identity;
using Projectopia.Models;

namespace Projectopia.Data;

public class SeedData
{
    public static async Task SeedUsersAndRolesAsync(IServiceProvider serviceProvider)
    {
        var roleManager = serviceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = serviceProvider.GetRequiredService<UserManager<User>>();

        
        // ROLE Section//
        string[] roleNames = ["Admin", "Student", "Supervisor"];

        foreach (var roleName in roleNames)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                await roleManager.CreateAsync(new IdentityRole(roleName));
            }
        }
        // End ROLE Section//

        // User Section//
        var adminEmail = "admin@example.com";
        var adminPassword = "Admin@123";

        if (await userManager.FindByEmailAsync(adminEmail) == null)
        {
            var adminUser = new Admin()
            {
                FullName = "admin@example.com",
                UserName = adminEmail,
                Email = adminEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(adminUser, adminPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(adminUser, "Admin");
            }
        }

        
        var userEmail = "student@example.com";
        var userPassword = "Student@123";

        if (await userManager.FindByEmailAsync(userEmail) == null)
        {
            var normalUser = new Student()
            {
                FullName = "student@example.com",
                UserName = userEmail,
                Email = userEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(normalUser, userPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(normalUser, "Student");
            }
            else
            {
                throw new Exception(result.Errors.First().Description);
            }
        }
        
        var supervisorEmail = "supervisor@example.com";
        var supervisorPassword = "Supervisor@123";

        if (await userManager.FindByEmailAsync(userEmail) == null)
        {
            var supervisor = new Supervisor()
            {
                FullName = "supervisor@example.com",
                UserName = userEmail,
                Email = userEmail,
                EmailConfirmed = true
            };

            var result = await userManager.CreateAsync(supervisor, userPassword);

            if (result.Succeeded)
            {
                await userManager.AddToRoleAsync(supervisor, "Supervisor");
            }
            else
            {
                throw new Exception(result.Errors.First().Description);
            }
        }
        // End User Section//
    }
}