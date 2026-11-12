using Microsoft.AspNetCore.Identity;
using MovieVault.Api.Domain;

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
}