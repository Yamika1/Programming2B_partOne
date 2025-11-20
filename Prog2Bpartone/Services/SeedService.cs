using Microsoft.AspNetCore.Identity;
using Prog2Bpartone.Data;
using Prog2Bpartone.Models;

namespace Prog2Bpartone.Services
{
    public class SeedService
    {
        public static async Task SeedDatabase(IServiceProvider serviceProvider)
        {
            using var scope = serviceProvider.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDBContext>();
            var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
            var userManager = scope.ServiceProvider.GetRequiredService<UserManager<Users>>();
            var logger = scope.ServiceProvider.GetRequiredService<ILogger<SeedService>>();

            try
            {
                await context.Database.EnsureCreatedAsync();

                string[] roles = { "Admin", "Lecturer", "Manager", "Super User" };
                foreach (var role in roles)
                    await CreateRole(roleManager, role);

                await CreateUser(userManager, logger,
                    fullName: "Shaolin Govender",
                    email: "admin@contractmonthlysystem.com",
                    password: "Admin@123",
                    role: "Admin"
                );

                await CreateUser(userManager, logger,
                    fullName: "Yamika Govender",
                    email: "manager@contractmonthlysystem.com",
                    password: "Manager@123",
                    role: "Manager"
                );

                await CreateUser(userManager, logger,
                    fullName: "Joe Miller",
                    email: "superUserr@contractmonthlysystem.com",
                    password: "SuperUser@123",
                    role: "Super User"
                );

                await CreateUser(userManager, logger,
                    fullName: "Sally Peterson",
                    email: "Lecturer@contractmonthlysystem.com",
                    password: "Lecturer@123",
                    role: "Lecturer"
                );
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "An error occurred while seeding the database.");
            }
        }

        private static async Task CreateRole(RoleManager<IdentityRole> roleManager, string roleName)
        {
            if (!await roleManager.RoleExistsAsync(roleName))
            {
                var result = await roleManager.CreateAsync(new IdentityRole(roleName));
                if (!result.Succeeded)
                    throw new Exception($"Failed to create role '{roleName}': {string.Join(", ", result.Errors.Select(e => e.Description))}");
            }
        }

        private static async Task CreateUser(
            UserManager<Users> userManager,
            ILogger logger,
            string fullName,
            string email,
            string password,
            string role)
        {
            var existingUser = await userManager.FindByEmailAsync(email);

            if (existingUser != null)
                return;

            var user = new Users
            {
                FullName = fullName,
                UserName = email,
                Email = email,
                NormalizedEmail = email.ToUpper(),
                NormalizedUserName = email.ToUpper(),
                EmailConfirmed = true,
                SecurityStamp = Guid.NewGuid().ToString()
            };

            var createResult = await userManager.CreateAsync(user, password);

            if (!createResult.Succeeded)
                return;

            await userManager.AddToRoleAsync(user, role);
        }
    }
}