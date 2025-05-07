using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
namespace Rakais_EL3lag.Seed
{
    

    public static class DbSeeder
    {
        public static async Task SeedAdminAsync(IServiceProvider serviceProvider)
        {

            using var scope = serviceProvider.CreateScope();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

            // Ensure Admin role exists
            if (!await roleManager.RoleExistsAsync("Admin"))
            {
                await roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            // Seed admin user
            var admin = await userManager.FindByNameAsync("Admin@Rakaiz");
            if (admin == null)
            {
                var adminUser = new IdentityUser
                {
                    UserName = "Admin@Rakaiz",
                  
                };

                var result = await userManager.CreateAsync(adminUser, "Admin@123");
                if (result.Succeeded)
                {
                    await userManager.AddToRoleAsync(adminUser, "Admin");
                }
            }
        }
    }
}
