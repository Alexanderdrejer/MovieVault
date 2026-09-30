using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using MovieVault.Api.Data;
using MovieVault.Api.Domain.DTOs;
using MovieVault.Api.Domain.Models;

namespace MovieVault.Api.Controllers;

[ApiController]
[Route("api/[controller]")]
public class MoviesController : ControllerBase
{
    private readonly MovieVaultContext _context;

    public MoviesController(MovieVaultContext context)
    {
        _context = context;
    }

    [HttpGet]
    [ProducesResponseType(typeof(List<MovieOverviewDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MovieOverviewDto>>> GetMovies()
    {
        return await _context.Movies
            .OrderBy(m => m.Title)
            .Select(m => new MovieOverviewDto
            {
                MovieId = m.MovieId,
                Title = m.Title,
                ReleaseDate = m.ReleaseDate,
                Status = m.Status,
                DirectorName = m.Director != null ? m.Director.Name : null,
                ProductionCompanyName = m.ProductionCompany != null ? m.ProductionCompany.Name : null,
                ProductionBudget = m.ProductionRecord != null ? m.ProductionRecord.ProductionBudget : null
            })
            .ToListAsync();
    }

    [HttpGet("{id:int}")]
    [ProducesResponseType(typeof(MovieDetailsDto), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<MovieDetailsDto>> GetMovie(int id)
    {
        var movie = await GetMovieDetails(id);
        if (movie == null)
        {
            return NotFound();
        }

        return movie;
    }

    [HttpGet("{id:int}/screenings")]
    [ProducesResponseType(typeof(List<ScreeningDto>), StatusCodes.Status200OK)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult<List<ScreeningDto>>> GetMovieScreenings(int id)
    {
        if (!await _context.Movies.AnyAsync(m => m.MovieId == id))
        {
            return NotFound();
        }

        return await _context.Screenings
            .Where(s => s.MovieId == id)
            .OrderBy(s => s.StartsAt)
            .Select(ScreeningDto.Projection)
            .ToListAsync();
    }

    [HttpGet("ticket-sales")]
    [ProducesResponseType(typeof(List<MovieTicketSalesDto>), StatusCodes.Status200OK)]
    public async Task<ActionResult<List<MovieTicketSalesDto>>> GetTicketSalesByMovie()
    {
        return await _context.TicketSales
            .GroupBy(ts => new { ts.Screening.MovieId, ts.Screening.Movie.Title })
            .Select(g => new MovieTicketSalesDto
            {
                MovieId = g.Key.MovieId,
                Title = g.Key.Title,
                TicketsSold = g.Sum(ts => ts.TicketCount),
                TotalRevenue = g.Sum(ts => ts.TicketCount * ts.PricePerTicket)
            })
            .OrderByDescending(x => x.TotalRevenue)
            .ToListAsync();
    }

    [HttpPost]
    [ProducesResponseType(typeof(MovieDetailsDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    public async Task<ActionResult<MovieDetailsDto>> CreateMovie(SaveMovieDto dto)
    {
        await ValidateMovie(dto);
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        var movie = new Movie();
        ApplyMovie(movie, dto);

        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();

        return CreatedAtAction(nameof(GetMovie), new { id = movie.MovieId }, await GetMovieDetails(movie.MovieId));
    }

    [HttpPut("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> UpdateMovie(int id, SaveMovieDto dto)
    {
        var movie = await _context.Movies
            .Include(m => m.ProductionRecord)
            .FirstOrDefaultAsync(m => m.MovieId == id);

        if (movie == null)
        {
            return NotFound();
        }

        await ValidateMovie(dto);
        if (!ModelState.IsValid)
        {
            return ValidationProblem(ModelState);
        }

        ApplyMovie(movie, dto);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpDelete("{id:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult> DeleteMovie(int id)
    {
        var movie = await _context.Movies.FindAsync(id);
        if (movie == null)
        {
            return NotFound();
        }

        if (await _context.Screenings.AnyAsync(s => s.MovieId == id))
        {
            return Problem(
                title: "Movie has screenings",
                detail: "A movie with screenings cannot be deleted, because its ticket sales would be lost.",
                statusCode: StatusCodes.Status409Conflict);
        }

        _context.Movies.Remove(movie);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id:int}/actors")]
    [ProducesResponseType(typeof(CastMemberDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<CastMemberDto>> AddActor(int id, AddCastMemberDto dto)
    {
        if (!await _context.Movies.AnyAsync(m => m.MovieId == id))
        {
            return NotFound();
        }

        var actor = await _context.Actors.FindAsync(dto.ActorId);
        if (actor == null)
        {
            ModelState.AddModelError(nameof(dto.ActorId), $"Actor {dto.ActorId} does not exist.");
            return ValidationProblem(ModelState);
        }

        if (await _context.MovieActors.AnyAsync(ma => ma.MovieId == id && ma.ActorId == dto.ActorId))
        {
            return Problem(
                title: "Actor already in cast",
                detail: $"Actor {dto.ActorId} is already part of the cast of movie {id}.",
                statusCode: StatusCodes.Status409Conflict);
        }

        _context.MovieActors.Add(new MovieActor
        {
            MovieId = id,
            ActorId = actor.ActorId,
            CharacterName = dto.CharacterName,
            RoleType = dto.RoleType
        });
        await _context.SaveChangesAsync();

        var castMember = new CastMemberDto
        {
            ActorId = actor.ActorId,
            Name = actor.Name,
            CharacterName = dto.CharacterName,
            RoleType = dto.RoleType
        };

        return CreatedAtAction(nameof(GetMovie), new { id }, castMember);
    }

    [HttpDelete("{id:int}/actors/{actorId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemoveActor(int id, int actorId)
    {
        var movieActor = await _context.MovieActors.FindAsync(id, actorId);
        if (movieActor == null)
        {
            return NotFound();
        }

        _context.MovieActors.Remove(movieActor);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    [HttpPost("{id:int}/locations")]
    [ProducesResponseType(typeof(FilmingLocationDto), StatusCodes.Status201Created)]
    [ProducesResponseType(typeof(ValidationProblemDetails), StatusCodes.Status400BadRequest)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    [ProducesResponseType(typeof(ProblemDetails), StatusCodes.Status409Conflict)]
    public async Task<ActionResult<FilmingLocationDto>> AddLocation(int id, AddFilmingLocationDto dto)
    {
        if (!await _context.Movies.AnyAsync(m => m.MovieId == id))
        {
            return NotFound();
        }

        var location = await _context.Locations.FindAsync(dto.LocationId);
        if (location == null)
        {
            ModelState.AddModelError(nameof(dto.LocationId), $"Location {dto.LocationId} does not exist.");
            return ValidationProblem(ModelState);
        }

        if (await _context.MovieLocations.AnyAsync(ml => ml.MovieId == id && ml.LocationId == dto.LocationId))
        {
            return Problem(
                title: "Location already added",
                detail: $"Location {dto.LocationId} is already a filming location of movie {id}.",
                statusCode: StatusCodes.Status409Conflict);
        }

        _context.MovieLocations.Add(new MovieLocation
        {
            MovieId = id,
            LocationId = location.LocationId,
            ShootingDays = dto.ShootingDays
        });
        await _context.SaveChangesAsync();

        var filmingLocation = new FilmingLocationDto
        {
            LocationId = location.LocationId,
            Name = location.Name,
            City = location.City,
            Country = location.Country,
            LocationType = location.LocationType,
            ShootingDays = dto.ShootingDays
        };

        return CreatedAtAction(nameof(GetMovie), new { id }, filmingLocation);
    }

    [HttpDelete("{id:int}/locations/{locationId:int}")]
    [ProducesResponseType(StatusCodes.Status204NoContent)]
    [ProducesResponseType(StatusCodes.Status404NotFound)]
    public async Task<ActionResult> RemoveLocation(int id, int locationId)
    {
        var movieLocation = await _context.MovieLocations.FindAsync(id, locationId);
        if (movieLocation == null)
        {
            return NotFound();
        }

        _context.MovieLocations.Remove(movieLocation);
        await _context.SaveChangesAsync();

        return NoContent();
    }

    private async Task<MovieDetailsDto?> GetMovieDetails(int id)
    {
        return await _context.Movies
            .Where(m => m.MovieId == id)
            .Select(m => new MovieDetailsDto
            {
                MovieId = m.MovieId,
                Title = m.Title,
                ReleaseDate = m.ReleaseDate,
                RuntimeMinutes = m.RuntimeMinutes,
                AgeRating = m.AgeRating,
                Status = m.Status,
                Director = m.Director == null ? null : new DirectorDto
                {
                    DirectorId = m.Director.DirectorId,
                    Name = m.Director.Name,
                    Nationality = m.Director.Nationality
                },
                ProductionCompanyName = m.ProductionCompany != null ? m.ProductionCompany.Name : null,
                ProductionRecord = m.ProductionRecord == null ? null : new ProductionRecordDto
                {
                    ProductionBudget = m.ProductionRecord.ProductionBudget,
                    ShootingStartDate = m.ProductionRecord.ShootingStartDate,
                    ShootingEndDate = m.ProductionRecord.ShootingEndDate
                },
                Cast = m.MovieActors
                    .OrderBy(ma => ma.Actor.Name)
                    .Select(ma => new CastMemberDto
                    {
                        ActorId = ma.ActorId,
                        Name = ma.Actor.Name,
                        CharacterName = ma.CharacterName,
                        RoleType = ma.RoleType
                    })
                    .ToList(),
                FilmingLocations = m.MovieLocations
                    .OrderBy(ml => ml.Location.Name)
                    .Select(ml => new FilmingLocationDto
                    {
                        LocationId = ml.LocationId,
                        Name = ml.Location.Name,
                        City = ml.Location.City,
                        Country = ml.Location.Country,
                        LocationType = ml.Location.LocationType,
                        ShootingDays = ml.ShootingDays
                    })
                    .ToList()
            })
            .FirstOrDefaultAsync();
    }

    private async Task ValidateMovie(SaveMovieDto dto)
    {
        if (dto.DirectorId != null && !await _context.Directors.AnyAsync(d => d.DirectorId == dto.DirectorId))
        {
            ModelState.AddModelError(nameof(dto.DirectorId), $"Director {dto.DirectorId} does not exist.");
        }

        if (dto.ProductionCompanyId != null && !await _context.ProductionCompanies.AnyAsync(pc => pc.ProductionCompanyId == dto.ProductionCompanyId))
        {
            ModelState.AddModelError(nameof(dto.ProductionCompanyId), $"Production company {dto.ProductionCompanyId} does not exist.");
        }

        var record = dto.ProductionRecord;
        if (record?.ShootingStartDate != null && record.ShootingEndDate != null && record.ShootingEndDate < record.ShootingStartDate)
        {
            ModelState.AddModelError("ProductionRecord.ShootingEndDate", "ShootingEndDate cannot be before ShootingStartDate.");
        }
    }

    private static void ApplyMovie(Movie movie, SaveMovieDto dto)
    {
        movie.Title = dto.Title;
        movie.ReleaseDate = dto.ReleaseDate;
        movie.RuntimeMinutes = dto.RuntimeMinutes;
        movie.AgeRating = dto.AgeRating;
        movie.Status = dto.Status;
        movie.DirectorId = dto.DirectorId;
        movie.ProductionCompanyId = dto.ProductionCompanyId;

        if (dto.ProductionRecord == null)
        {
            movie.ProductionRecord = null;
            return;
        }

        movie.ProductionRecord ??= new ProductionRecord();
        movie.ProductionRecord.ProductionBudget = dto.ProductionRecord.ProductionBudget;
        movie.ProductionRecord.ShootingStartDate = dto.ProductionRecord.ShootingStartDate;
        movie.ProductionRecord.ShootingEndDate = dto.ProductionRecord.ShootingEndDate;
    }
}
