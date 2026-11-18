using Microsoft.AspNetCore.Identity;
using MovieVault.Api.Domain;
using MovieVault.Api.Domain.Models;

namespace MovieVault.Api.Data;

public static class IdentitySeeder // seed mit shit til databasen.
{
    public static async Task SeedRolesAsync(RoleManager<IdentityRole> roleManager) // Får rolemanager og skal oprette manglende roller.
    {
        foreach (var role in Roles.All) // For hver rolle i min statisk Rolle.cs klasse.
        {
            if (!await roleManager.RoleExistsAsync(role)) // his rollen ikke allerede findes i databasen, så..
            {
                await roleManager.CreateAsync(new IdentityRole(role)); // opret den.
            }
        }
    }

    public static async Task SeedUserAsync(UserManager<ApplicationUser> userManager, string userName, string password, string role)
    {
        var user = await userManager.FindByNameAsync(userName);

        if (user is null)
        {
            user = new ApplicationUser { UserName = userName };
            var result = await userManager.CreateAsync(user, password);

            if (!result.Succeeded)
            {
                var errors = string.Join(", ", result.Errors.Select(e => e.Description));
                throw new InvalidOperationException($"Kunne ikke oprette {userName}: {errors}");
            }
        }

        if (!await userManager.IsInRoleAsync(user, role))
        {
            await userManager.AddToRoleAsync(user, role);
        }
    }
}