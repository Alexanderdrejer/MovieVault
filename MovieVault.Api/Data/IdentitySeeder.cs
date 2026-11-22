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

    public static async Task LinkDomainUsersAsync(MovieVaultContext context, UserManager<ApplicationUser> userManager)
    {
        var actorUser = await userManager.FindByNameAsync("actor");
        var directorUser = await userManager.FindByNameAsync("director");
        var reviewerUser = await userManager.FindByNameAsync("reviewer");
        var reviewer2User = await userManager.FindByNameAsync("reviewer2");

        var actor = await context.Actors.FindAsync(1);
        var director = await context.Directors.FindAsync(1);
        var reviewer = await context.Reviewers.FindAsync(1);
        var reviewer2 = await context.Reviewers.FindAsync(2);

        if (actor != null && actorUser != null)
            actor.UserId = actorUser.Id;

        if (director != null && directorUser != null)
            director.UserId = directorUser.Id;

        if (reviewer != null && reviewerUser != null)
            reviewer.UserId = reviewerUser.Id;

        if (reviewer2 != null && reviewer2User != null)
            reviewer2.UserId = reviewer2User.Id;

        await context.SaveChangesAsync();
    }
}