using System.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieVault.Api.Data;
using MovieVault.Api.Domain.DTOs;
using MovieVault.Api.Domain.Models;

namespace MovieVault.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TicketSalesController : ControllerBase
{
    private readonly MovieVaultContext _context;

    public TicketSalesController(MovieVaultContext context)
    {
        _context = context;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<TicketSaleDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<TicketSaleDto>>> GetTicketSales()
    {
        return await _context.TicketSales
            .OrderBy(ts => ts.TicketSaleId)
            .Select(TicketSaleDto.Projection)
            .ToListAsync();
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(TicketSaleDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<TicketSaleDto>> GetTicketSale(int id)
    {
        var ticketSale = await _context.TicketSales
            .Where(ts => ts.TicketSaleId == id)
            .Select(TicketSaleDto.Projection)
            .FirstOrDefaultAsync();

        if (ticketSale == null)
        {
            return NotFound();
        }

        return ticketSale;
    }

    [HttpPost]
    [ProducesResponseType(typeof(TicketSaleDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<TicketSaleDto>> CreateTicketSale(CreateTicketSaleDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        var screening = await _context.Screenings
            .Include(s => s.Screen)
            .FirstOrDefaultAsync(s => s.ScreeningId == dto.ScreeningId);

        if (screening == null)
        {
            ModelState.AddModelError(nameof(dto.ScreeningId), $"Screening {dto.ScreeningId} does not exist.");
            return ValidationProblem(ModelState);
        }

        if (screening.IsCancelled)
        {
            return Problem(
                title: "Screening is cancelled",
                detail: $"Screening {screening.ScreeningId} is cancelled, so no tickets can be sold.",
                statusCode: StatusCodes.Status409Conflict);
        }

        if (screening.StartsAt <= DateTime.Now)
        {
            return Problem(
                title: "Screening has started",
                detail: $"Screening {screening.ScreeningId} started at {screening.StartsAt:yyyy-MM-dd HH:mm}, so no tickets can be sold.",
                statusCode: StatusCodes.Status409Conflict);
        }

        var seatsProblem = await CheckCapacity(screening, dto.TicketCount!.Value, null);
        if (seatsProblem != null)
        {
            return Problem(
                title: "Not enough seats",
                detail: seatsProblem,
                statusCode: StatusCodes.Status409Conflict);
        }

        var ticketSale = new TicketSale
        {
            ScreeningId = screening.ScreeningId,
            SaleDateTime = DateTime.Now,
            TicketCount = dto.TicketCount.Value,
            PricePerTicket = dto.PricePerTicket ?? screening.TicketPrice
        };

        _context.TicketSales.Add(ticketSale);
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        var created = await _context.TicketSales
            .Where(ts => ts.TicketSaleId == ticketSale.TicketSaleId)
            .Select(TicketSaleDto.Projection)
            .FirstAsync();

        return CreatedAtAction(nameof(GetTicketSale), new { id = ticketSale.TicketSaleId }, created);
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> UpdateTicketSale(int id, UpdateTicketSaleDto dto)
    {
        using var transaction = await _context.Database.BeginTransactionAsync(IsolationLevel.Serializable);

        var ticketSale = await _context.TicketSales
            .Include(ts => ts.Screening)
            .ThenInclude(s => s.Screen)
            .FirstOrDefaultAsync(ts => ts.TicketSaleId == id);

        if (ticketSale == null)
        {
            return NotFound();
        }

        var seatsProblem = await CheckCapacity(ticketSale.Screening, dto.TicketCount!.Value, ticketSale.TicketSaleId);
        if (seatsProblem != null)
        {
            return Problem(
                title: "Not enough seats",
                detail: seatsProblem,
                statusCode: StatusCodes.Status409Conflict);
        }

        ticketSale.TicketCount = dto.TicketCount.Value;
        ticketSale.PricePerTicket = dto.PricePerTicket!.Value;
        await _context.SaveChangesAsync();
        await transaction.CommitAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> DeleteTicketSale(int id)
    {
        var ticketSale = await _context.TicketSales.FindAsync(id);
        if (ticketSale == null)
        {
            return NotFound();
        }

        _context.TicketSales.Remove(ticketSale);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<string?> CheckCapacity(Screening screening, int ticketCount, int? excludeTicketSaleId)
    {
        var alreadySold = await _context.TicketSales
            .Where(ts => ts.ScreeningId == screening.ScreeningId && ts.TicketSaleId != excludeTicketSaleId)
            .SumAsync(ts => ts.TicketCount);

        var seatsLeft = screening.Screen.SeatingCapacity - alreadySold;
        if (ticketCount <= seatsLeft)
        {
            return null;
        }

        return $"Screening {screening.ScreeningId} has {seatsLeft} seats left, but {ticketCount} tickets were requested.";
    }
}
