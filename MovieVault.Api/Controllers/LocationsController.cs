using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieVault.Api.Data;
using MovieVault.Api.Domain.DTOs;

namespace MovieVault.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LocationsController : ControllerBase
{
    private readonly MovieVaultContext _context;

    public LocationsController(MovieVaultContext context)
    {
        _context = context;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<LocationDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<LocationDto>>> GetLocations()
    {
        return await _context.Locations
            .OrderBy(l => l.Name)
            .Select(l => new LocationDto
            {
                LocationId = l.LocationId,
                Name = l.Name,
                City = l.City,
                Country = l.Country,
                LocationType = l.LocationType
            })
            .ToListAsync();
    }
}
