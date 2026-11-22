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
public class CinemasController : ControllerBase
{
    private readonly MovieVaultContext _context;
    private readonly MovieAccessService _access;

    public CinemasController(MovieVaultContext context, MovieAccessService access)
    {
        _context = context;
        _access = access;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<CinemaDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<CinemaDto>>> GetCinemas()
    {
        return await _context.Cinemas
            .OrderBy(c => c.Name)
            .Select(c => new CinemaDto
            {
                CinemaId = c.CinemaId,
                Name = c.Name,
                City = c.City,
                StreetAddress = c.StreetAddress,
                Screens = c.Screens
                    .OrderBy(s => s.ScreenNumberOrName)
                    .Select(s => new ScreenDto
                    {
                        ScreenId = s.ScreenId,
                        ScreenNumberOrName = s.ScreenNumberOrName,
                        SeatingCapacity = s.SeatingCapacity,
                        ScreenType = s.ScreenType
                    })
                    .ToList()
            })
            .ToListAsync();
    }

    [HttpGet("{id:int}/screenings")]
    [ProducesResponseType(typeof(List<ScreeningDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<ScreeningDto>>> GetCinemaScreenings(int id)
    {
        if (!await _context.Cinemas.AnyAsync(c => c.CinemaId == id))
        {
            return NotFound();
        }

        var allowed = await _access.GetAllowedMovieIdsAsync(User);

        return await _context.Screenings
            .Where(s => s.Screen.CinemaId == id)
            .Where(s => allowed == null || allowed.Contains(s.MovieId))
            .OrderBy(s => s.StartsAt)
            .Select(ScreeningDto.Projection)
            .ToListAsync();
    }
}
