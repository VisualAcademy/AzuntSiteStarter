using Microsoft.AspNetCore.Identity;

namespace Azunt.Web.Data;

public static class SeedData
{
    public const string TestEmail = "demo@azunt.local";
    public const string TestPassword = "Azunt123!";

    public static async Task InitializeAsync(IServiceProvider services)
    {
        using var scope = services.CreateScope();

        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<IdentityUser>>();

        const string administratorRole = "Administrator";

        if (!await roleManager.RoleExistsAsync(administratorRole))
        {
            var roleResult = await roleManager.CreateAsync(new IdentityRole(administratorRole));
            if (!roleResult.Succeeded)
            {
                throw new InvalidOperationException(string.Join("; ", roleResult.Errors.Select(error => error.Description)));
            }
        }

        var user = await userManager.FindByEmailAsync(TestEmail);
        if (user is null)
        {
            user = new IdentityUser
            {
                UserName = TestEmail,
                Email = TestEmail,
                EmailConfirmed = true
            };

            var createResult = await userManager.CreateAsync(user, TestPassword);
            if (!createResult.Succeeded)
            {
                throw new InvalidOperationException(string.Join("; ", createResult.Errors.Select(error => error.Description)));
            }
        }

        if (!await userManager.IsInRoleAsync(user, administratorRole))
        {
            await userManager.AddToRoleAsync(user, administratorRole);
        }
    }
}
