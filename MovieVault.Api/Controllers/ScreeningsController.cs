using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieVault.Api.Data;
using MovieVault.Api.Domain.DTOs;
using MovieVault.Api.Domain.Models;

namespace MovieVault.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class ScreeningsController : ControllerBase
{
    private readonly MovieVaultContext _context;

    public ScreeningsController(MovieVaultContext context)
    {
        _context = context;
    }

    [HttpGet("upcoming")]
    [ProducesResponseType(typeof(List<ScreeningDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<ScreeningDto>>> GetUpcomingScreenings()
    {
        return await _context.Screenings
            .Where(s => s.StartsAt >= DateTime.Now && !s.IsCancelled)
            .OrderBy(s => s.StartsAt)
            .Select(ScreeningDto.Projection)
            .ToListAsync();
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(ScreeningDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<ScreeningDto>> GetScreening(int id)
    {
        var screening = await _context.Screenings
            .Where(s => s.ScreeningId == id)
            .Select(ScreeningDto.Projection)
            .FirstOrDefaultAsync();

        if (screening == null)
        {
            return NotFound();
        }

        return screening;
    }

    [HttpPost]
    [ProducesResponseType(typeof(ScreeningDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<ScreeningDto>> CreateScreening(CreateScreeningDto dto)
    {
        if (!await _context.Movies.AnyAsync(m => m.MovieId == dto.MovieId))
        {
            ModelState.AddModelError(nameof(dto.MovieId), $"Movie {dto.MovieId} does not exist.");
        }

        if (!await _context.Screens.AnyAsync(s => s.ScreenId == dto.ScreenId))
        {
            ModelState.AddModelError(nameof(dto.ScreenId), $"Screen {dto.ScreenId} does not exist.");
        }

        if (dto.StartsAt <= DateTime.Now)
        {
            ModelState.AddModelError(nameof(dto.StartsAt), "A new screening must start in the future.");
        }

        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var screening = new Screening
        {
            MovieId = dto.MovieId!.Value,
            ScreenId = dto.ScreenId!.Value,
            StartsAt = dto.StartsAt!.Value,
            TicketPrice = dto.TicketPrice!.Value
        };

        _context.Screenings.Add(screening);
        await _context.SaveChangesAsync();

        var created = await _context.Screenings
            .Where(s => s.ScreeningId == screening.ScreeningId)
            .Select(ScreeningDto.Projection)
            .FirstAsync();

        return CreatedAtAction(nameof(GetScreening), new { id = screening.ScreeningId }, created);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> UpdateScreening(int id, UpdateScreeningDto dto)
    {
        var screening = await _context.Screenings.FindAsync(id);
        if (screening == null)
        {
            return NotFound();
        }

        var screen = await _context.Screens.FindAsync(dto.ScreenId);
        if (screen == null)
        {
            ModelState.AddModelError(nameof(dto.ScreenId), $"Screen {dto.ScreenId} does not exist.");
            return ValidationProblem(ModelState);
        }

        var ticketsSold = await _context.TicketSales
            .Where(ts => ts.ScreeningId == id)
            .SumAsync(ts => ts.TicketCount);

        if (ticketsSold > screen.SeatingCapacity)
        {
            return Problem(
                title: "Screen too small",
                detail: $"{ticketsSold} tickets are already sold, but screen {screen.ScreenId} only has {screen.SeatingCapacity} seats.",
                statusCode: StatusCodes.Status409Conflict);
        }

        screening.ScreenId = screen.ScreenId;
        screening.StartsAt = dto.StartsAt!.Value;
        screening.TicketPrice = dto.TicketPrice!.Value;
        screening.IsCancelled = dto.IsCancelled;
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> DeleteScreening(int id)
    {
        var screening = await _context.Screenings.FindAsync(id);
        if (screening == null)
        {
            return NotFound();
        }

        if (await _context.TicketSales.AnyAsync(ts => ts.ScreeningId == id))
        {
            return Problem(
                title: "Screening has ticket sales",
                detail: "A screening with ticket sales cannot be deleted. Cancel it with PUT and IsCancelled = true instead.",
                statusCode: StatusCodes.Status409Conflict);
        }

        _context.Screenings.Remove(screening);
        await _context.SaveChangesAsync();

        return NoContent();
    }
}
