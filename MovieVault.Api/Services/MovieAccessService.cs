using System.Security.Claims;
using Microsoft.EntityFrameworkCore;
using MovieVault.Api.Data;
using MovieVault.Api.Domain;

namespace MovieVault.Api.Services;

public class MovieAccessService
{
    private readonly MovieVaultContext _context;

    public MovieAccessService(MovieVaultContext context)
    {
        _context = context;
    }

    public async Task<List<int>?> GetAllowedMovieIdsAsync(ClaimsPrincipal user)
    {
        if (user.IsInRole(Roles.Admin) || user.IsInRole(Roles.Reviewer))
            return null;

        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        var movieIds = new List<int>();

        if (user.IsInRole(Roles.Actor))
            movieIds.AddRange(await _context.MovieActors
                .Where(ma => ma.Actor.UserId == userId)
                .Select(ma => ma.MovieId)
                .ToListAsync());

        if (user.IsInRole(Roles.Director))
            movieIds.AddRange(await _context.Movies
                .Where(m => m.Director != null && m.Director.UserId == userId)
                .Select(m => m.MovieId)
                .ToListAsync());

        return movieIds.Distinct().ToList();
    }

    public async Task<bool> CanAccessMovieAsync(ClaimsPrincipal user, int movieId)
    {
        var allowed = await GetAllowedMovieIdsAsync(user);
        return allowed == null || allowed.Contains(movieId);
    }

    public async Task<int?> GetActorIdAsync(ClaimsPrincipal user)
    {
        var userId = user.FindFirstValue(ClaimTypes.NameIdentifier);
        return await _context.Actors
            .Where(a => a.UserId == userId)
            .Select(a => (int?)a.ActorId)
            .FirstOrDefaultAsync();
    }
}
