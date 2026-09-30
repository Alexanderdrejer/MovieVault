using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieVault.Api.Data;
using MovieVault.Api.Domain.DTOs;

namespace MovieVault.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class CinemasController : ControllerBase
{
    private readonly MovieVaultContext _context;

    public CinemasController(MovieVaultContext context)
    {
        _context = context;
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

        return await _context.Screenings
            .Where(s => s.Screen.CinemaId == id)
            .OrderBy(s => s.StartsAt)
            .Select(ScreeningDto.Projection)
            .ToListAsync();
    }
}
