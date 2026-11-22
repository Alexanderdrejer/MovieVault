using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieVault.Api.Data;
using MovieVault.Api.Domain;
using MovieVault.Api.Domain.DTOs;
using MovieVault.Api.Services;

namespace MovieVault.Api.Controllers;

[ApiController]
[Authorize]
[Route("api/[controller]")]
public class ActorsController : ControllerBase
{
    private readonly MovieVaultContext _context;
    private readonly MovieAccessService _access;

    public ActorsController(MovieVaultContext context, MovieAccessService access)
    {
        _context = context;
        _access = access;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<ActorDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ActorDto>>> GetActors()
    {
        return await _context.Actors
            .OrderBy(a => a.Name)
            .Select(a => new ActorDto
            {
                ActorId = a.ActorId,
                Name = a.Name,
                DateOfBirth = a.DateOfBirth,
                Nationality = a.Nationality,
                Gender = a.Gender
            })
            .ToListAsync();
    }

    [Authorize(Roles = $"{Roles.Actor},{Roles.Admin}")]
    [HttpGet("{id:int}/performance")]
    [ProducesResponseType(typeof(ActorPerformanceDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ActorPerformanceDto>> GetActorPerformance(int id)
    {
        var performance = await _context.Actors
            .Where(a => a.ActorId == id)
            .Select(a => new ActorPerformanceDto
            {
                ActorId = a.ActorId,
                Name = a.Name,
                Movies = a.MovieActors
                    .OrderBy(ma => ma.Movie.Title)
                    .Select(ma => new ActorMovieDto
                    {
                        MovieId = ma.MovieId,
                        Title = ma.Movie.Title,
                        CharacterName = ma.CharacterName,
                        RoleType = ma.RoleType,
                        TicketsSold = ma.Movie.Screenings
                            .SelectMany(s => s.TicketSales)
                            .Sum(ts => ts.TicketCount),
                        Revenue = ma.Movie.Screenings
                            .SelectMany(s => s.TicketSales)
                            .Sum(ts => ts.TicketCount * ts.PricePerTicket)
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();

        if (performance == null)
        {
            return NotFound();
        }

        if (!User.IsInRole(Roles.Admin) && await _access.GetActorIdAsync(User) != id)
        {
            return Forbid();
        }

        performance.TotalTicketsSold = performance.Movies.Sum(m => m.TicketsSold);
        performance.TotalRevenue = performance.Movies.Sum(m => m.Revenue);

        return performance;
    }
}
