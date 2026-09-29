using System.ComponentModel.DataAnnotations;

namespace MovieVault.Api.Domain.DTOs;

public class MovieOverviewDto
{
    public int MovieId { get; set; }
    public string Title { get; set; } = "";
    public DateTime? ReleaseDate { get; set; }
    public string Status { get; set; } = "";
    public string? DirectorName { get; set; }
    public string? ProductionCompanyName { get; set; }
    public decimal? ProductionBudget { get; set; }
}

public class MovieDetailsDto
{
    public int MovieId { get; set; }
    public string Title { get; set; } = "";
    public DateTime? ReleaseDate { get; set; }
    public int? RuntimeMinutes { get; set; }
    public string? AgeRating { get; set; }
    public string Status { get; set; } = "";
    public DirectorDto? Director { get; set; }
    public string? ProductionCompanyName { get; set; }
    public ProductionRecordDto? ProductionRecord { get; set; }
    public List<CastMemberDto> Cast { get; set; } = new();
    public List<FilmingLocationDto> FilmingLocations { get; set; } = new();
}

public class DirectorDto
{
    public int DirectorId { get; set; }
    public string Name { get; set; } = "";
    public string? Nationality { get; set; }
}

public class ProductionRecordDto
{
    [Range(0, 1000000000000)]
    public decimal? ProductionBudget { get; set; }

    public DateTime? ShootingStartDate { get; set; }
    public DateTime? ShootingEndDate { get; set; }
}

public class CastMemberDto
{
    public int ActorId { get; set; }
    public string Name { get; set; } = "";
    public string? CharacterName { get; set; }
    public string RoleType { get; set; } = "";
}

public class FilmingLocationDto
{
    public int LocationId { get; set; }
    public string Name { get; set; } = "";
    public string? City { get; set; }
    public string? Country { get; set; }
    public string LocationType { get; set; } = "";
    public int? ShootingDays { get; set; }
}

public class SaveMovieDto
{
    [Required]
    [StringLength(150)]
    public string Title { get; set; } = "";

    public DateTime? ReleaseDate { get; set; }

    [Range(1, 1000)]
    public int? RuntimeMinutes { get; set; }

    [StringLength(10)]
    public string? AgeRating { get; set; }

    [Required]
    [RegularExpression("^(planned|in production|released|archived)$",
        ErrorMessage = "Status must be one of: planned, in production, released, archived.")]
    public string Status { get; set; } = "";

    public int? DirectorId { get; set; }
    public int? ProductionCompanyId { get; set; }
    public ProductionRecordDto? ProductionRecord { get; set; }
}

public class AddCastMemberDto
{
    [Required]
    public int? ActorId { get; set; }

    [StringLength(100)]
    public string? CharacterName { get; set; }

    [Required]
    [RegularExpression("^(lead|supporting|cameo)$",
        ErrorMessage = "RoleType must be one of: lead, supporting, cameo.")]
    public string RoleType { get; set; } = "";
}

public class AddFilmingLocationDto
{
    [Required]
    public int? LocationId { get; set; }

    [Range(1, 1000)]
    public int? ShootingDays { get; set; }
}

public class MovieTicketSalesDto
{
    public int MovieId { get; set; }
    public string Title { get; set; } = "";
    public int TicketsSold { get; set; }
    public decimal TotalRevenue { get; set; }
}
